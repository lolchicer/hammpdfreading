using iTextSharp.text.pdf;
using iTextSharp.text.pdf.events;
using iTextSharp.text.pdf.parser;
using System;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Infor.HammPdfReading
{
    public class Reader
    {
        public static class Regexes
        {
            public const string Table =
                "(?<=Ед\\.\n)" +
                "(.|\n)*" +
                "(?=\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2})";

            public const string Item = "[0-9]+(\\.[0-9]{2})?";
            public const string PartNo = "[0-9]+";
            public const string ValidFor = "[0-9]{1,4}-[0-9]{1,4}";
            public const string Quantity = "[0-9]+";
            public const string Unit = "[A-Z]{1,2}";
            public const string DesignationSpace = "[^А-я]*";
            public const string DesignationRussian = ".+";

            public const string Assembly = "[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}";
            public const string Date = "[0-9]{2}\\.[0-9]{2}\\.[0-9]{4}";
            public const string AssemblyDesignations = "(.|\n)*(?=Benennung:)";
            public const string PageInfo5 = "ряд:";
            public const string Series = ".*";

            public static string[] TableLineToArray() => new[] {
                Item,
                PartNo,
                ValidFor,
                Quantity,
                Unit,
                DesignationSpace,
                DesignationRussian
            };

            public static string[] MainToArray() => new[]
            {
                Item,
                PartNo,
                ValidFor,
                Quantity,
                Unit
            };

            public static string[] DesignationsToArray() => new[]
            {
                DesignationSpace,
                DesignationRussian
            };

            public static string[] PageInfoToArray() => new[]
            {
                Assembly,
                PartNo,
                Date,
                AssemblyDesignations,
                PageInfo5,
                Series
            };
        }

        static string[] ContinuousMatch(string text, string[] regexes)
        {
            var value = new List<string>();

            var textCut = text.Trim();
            foreach (var regex in regexes)
            {
                var match = Regex.Match(textCut, regex);
                var item = match.Value;
                item = item.Trim();
                value.Add(item);
                textCut = textCut.Remove(0, match.Index + match.Length);
                textCut = textCut.Trim();
            }

            return value.ToArray();
        }

        static string DesignationRussian(string text)
        {
            var designations = ContinuousMatch(text, Regexes.DesignationsToArray());
            if (designations[1] == string.Empty)
            {
                // гигантское количество функций, разворачивающих строку
                var reversed = text.ToCharArray();
                Array.Reverse(reversed);
                var reversedMatched = Regex.Match(new string(reversed), "((.|\\n)*)(.|\\n)*\\1");
                var matched = reversedMatched.Groups[0].Value.ToCharArray();
                Array.Reverse(matched);
                return new string(matched) ?? string.Empty;
            }
            else
                return designations[1];
        }

        static string DesignationRussian(string[] lines)
        {
            var text = string.Join(" ", lines);
            return DesignationRussian(text.Trim());
        }

        static string DesignationRussianLines(string text)
        {
            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Trim();
            return DesignationRussian(string.Concat(lines));
        }

        static string DesignationRussianLines(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Trim();
            return DesignationRussian(string.Concat(lines));
        }

        enum RowType
        {
            full,
            main,
            designation
        }

        static Detail[] Details(string text)
        {
            var details = new List<Detail>();

            var textCut = Regex.Match(text, Regexes.Table).Value.Trim();
            var isFinished = true;

            RowType rowType;

            var designations = new List<string>();

            foreach (var line in textCut.Split('\n'))
            {
                var row = ContinuousMatch(line, Regexes.TableLineToArray());
                if (!row.Contains(string.Empty))
                    rowType = RowType.full;
                else
                {
                    row = ContinuousMatch(line, Regexes.MainToArray());
                    if (!row.Contains(string.Empty))
                        rowType = RowType.main;
                    else
                    {
                        row = ContinuousMatch(line, Regexes.DesignationsToArray());
                        rowType = RowType.designation;
                    }
                }

                if (rowType == RowType.designation)
                    designations.Add(line);
                else
                {
                    if (!isFinished)
                    {
                        var i = details.Count - 1;
                        var replacing = details[i];
                        replacing.Designation += DesignationRussian(designations.ToArray());
                        replacing.Designation = replacing.Designation.Trim();
                        details.RemoveAt(i);
                        details.Add(replacing);

                        isFinished = true;
                    }

                    if (rowType == RowType.full)
                        details.Add(Detail.FromFields(row));
                    if (rowType == RowType.main)
                    {
                        row = ContinuousMatch(line, Regexes.MainToArray());
                        var extendedRow = new string[row.Length + 2];
                        row.CopyTo(extendedRow, 0);
                        new[] { string.Empty, string.Empty }.CopyTo(extendedRow, 5);
                        details.Add(Detail.FromFields(extendedRow));

                        isFinished = false;
                    }
                }
            }

            return details.ToArray();
        }

        static ExtendedDetail[] ExtendedDetails(string text)
        {
            var details = new List<ExtendedDetail>();

            var pageInfo = ContinuousMatch(
                Regex.Match(text, "\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}(.|\n)*").Value,
                Regexes.PageInfoToArray());
            var assembly = Convert.ToInt32(pageInfo[1]);

            foreach (var detail in Details(text))
                details.Add(new ExtendedDetail()
                {
                    Item = detail.Item,
                    PartNo = detail.PartNo,
                    ValidFor = detail.ValidFor,
                    Quantity = detail.Quantity,
                    Unit = detail.Unit,
                    Designation = detail.Designation,
                    Assembly = assembly
                });

            return details.ToArray();
        }

        static bool IsTablePage(string text) => Regex.IsMatch(text, Regexes.Table);

        PdfReader _reader;

        public Detail[] Details(int page)
        {
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(_reader, page, strategy);
            var match = Regex.Match(text, Regexes.Table);

            return Details(match.Value);
        }

        public ExtendedDetail[] ExtendedDetails(int page)
        {
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(_reader, page, strategy);

            return ExtendedDetails(text);
        }

        public ExtendedDetail[] ExtendedDetails(int page, int count)
        {
            var details = new List<ExtendedDetail>();
            for (int i = 0; i < count; i++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(_reader, page + i, strategy);
                if (IsTablePage(text))
                    details.AddRange(ExtendedDetails(text));
            }
            return details.ToArray();
        }

        public ExtendedDetail[] ExtendedDetails() => ExtendedDetails(1, _reader.NumberOfPages);

        public Module GetModule(string text)
        {
            var pageInfo = ContinuousMatch(
                Regex.Match(text, "\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}(.|\n)*").Value,
                Regexes.PageInfoToArray());

            return new Module() { No = Convert.ToInt32(pageInfo[1]), Assembly = pageInfo[0], Series = pageInfo[5], Designation = DesignationRussianLines(pageInfo[3]) };
        }

        public Module GetModule(int page)
        {
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(_reader, page, strategy);

            return GetModule(text);
        }

        public Module[] GetModules(int page, int count)
        {
            var modules = new List<Module>();
            for (int i = 0; i < count; i++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(_reader, page + i, strategy);
                if (IsTablePage(text))
                    modules.Add(GetModule(text));
            }
            return modules.ToArray();
        }

        public Module[] GetModules()
        {
            return GetModules(1, _reader.NumberOfPages);
        }

        public Reader(PdfReader reader)
        {
            _reader = reader;
        }
    }
}