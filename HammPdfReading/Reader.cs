using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
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
            public const string DesignationSpace = "[^А-я]+";
            public const string DesignationRussian = ".+";

            public static string[] ToArray() => new[] {
                Item,
                PartNo,
                ValidFor,
                Quantity,
                Unit,
                DesignationSpace,
                DesignationRussian
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

        static string[] PageInfo(string text) => ContinuousMatch(text, new[]
            {
                "[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}",
                Regexes.PartNo,
                "[0-9]{2}\\.[0-9]{2}\\.[0-9]{4}",
                Regexes.DesignationSpace,
                Regexes.DesignationRussian,
                "ряд:",
                ".*"
            });

        static string Field(string line, string regex) => Regex.Match(line, regex).Value.Trim();

        // Regexes.DesignationRussian почему-то не считает первую букву "С" в "СФЕРО-ЦИЛИНДРИЧЕСКОЙ" на странице 215
        static string[] Fields(string line) => ContinuousMatch(line, Regexes.ToArray());

        static string[] Designations(string line) =>
            new[]
            {
                Field(line, Regexes.DesignationSpace),
                Field(line, Regexes.DesignationRussian)
            };

        static Detail[] Details(string text)
        {
            var details = new List<Detail>();

            text = Regex.Match(text, Regexes.Table).Value;

            foreach (var line in text.Split('\n'))
            {
                var row = Fields(line);
                if (row[1] != string.Empty)
                {
                    details.Add(Detail.FromFields(row));
                }
                else
                {
                    var i = details.Count - 1;
                    var replacing = details[i];
                    replacing.Designation += ' ' + row[6];
                    replacing.Designation = replacing.Designation.Trim();
                    details.RemoveAt(i);
                    details.Add(replacing);
                }
            }

            return details.ToArray();
        }

        static ExtendedDetail[] ExtendedDetails(string text)
        {
            var details = new List<ExtendedDetail>();

            var pageInfo = PageInfo(Regex.Match(text, "\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}(.|\n)*").Value);
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
            var pageInfo = PageInfo(Regex.Match(text, "\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}(.|\n)*").Value);

            return new Module() { No = Convert.ToInt32(pageInfo[1]), Assembly = pageInfo[0], Series = pageInfo[4], Designation = pageInfo[6] };
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

        public Reader(PdfReader reader)
        {
            _reader = reader;
        }
    }
}