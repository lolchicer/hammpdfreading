using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace Infor.HammPdfReading
{
    public static class Reader
    {
        static List<string> ContinuousMatch(string text, List<string> regexes)
        {
            var value = new List<string>();

            var textCut = text;
            foreach (var regex in regexes)
            {
                var item = Regex.Match(textCut, regex).Value;
                item = item.Trim();
                value.Add(item);
                textCut = textCut.Trim();
                textCut = textCut.Remove(0, item.Length);
            }

            return value;
        }

        public static class Regexes
        {
            public const string Item = "[0-9]+(\\.[0-9]{2})?";
            public const string PartNo = "[0-9]+";
            public const string ValidFor = "[0-9]{1,4}-[0-9]{1,4}";
            public const string Quantity = "[0-9]+";
            public const string Unit = "[A-Z]{1,2}";
            public const string DesignationSpace = "[^А-я]+";
            public const string DesignationRussian = ".+";

            public static List<string> List() => new List<string>() {
                Item,
                PartNo,
                ValidFor,
                Quantity,
                Unit,
                DesignationSpace,
                DesignationRussian
            };
        }

        static string Field(string line, string regex)
        {
            string field;
            field = Regex.Match(line, regex).Value;
            field = field.Trim();
            return field;
        }

        // _regexes[6] почему-то не считает первую букву "С" в "СФЕРО-ЦИЛИНДРИЧЕСКОЙ" на странице 215
        public static List<string> Fields(string line)
        {
            var fields = new List<string>();

            var lineCut = line;
            string field;
            foreach (var regex in Regexes.List())
            {
                field = Regex.Match(lineCut, regex).Value;
                field = field.Trim();
                fields.Add(field);
                lineCut = lineCut.Trim();
                lineCut = lineCut.Remove(0, field.Length);
            };

            return fields;
        }

        public static List<string> Designations(string line) =>
            new List<string>()
            {
                Field(line, Regexes.DesignationSpace),
                Field(line, Regexes.DesignationRussian)
            };

        public static List<Detail> Details(PdfReader reader, int page)
        {
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
            var match = Regex.Match(text,
                "(?<=Ед\\.\n)" +
                "(.|\n)*" +
                "(?=\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2})");

            return Details(match.Value);
        }

        public static List<Detail> Details(string text)
        {
            var details = new List<Detail>();

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

            return details;
        }

        public static List<string> PageInfo(string text) => ContinuousMatch(text, new List<string>(new[]
            {
                "[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}",
                Regexes.PartNo,
                "[0-9]{2}\\.[0-9]{2}\\.[0-9]{4}",
                "[^ ]*",
                ".*"
            }));

        public static List<ExtendedDetail> ExtendedDetails(PdfReader reader, int page)
        {
            var strategy = new SimpleTextExtractionStrategy();
            var text = PdfTextExtractor.GetTextFromPage(reader, page, strategy);

            var details = new List<ExtendedDetail>();

            var pageInfo = PageInfo(Regex.Match(text, "\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}(.|\n)*").Value);
            var assembly = Convert.ToInt32(pageInfo[1]);

            foreach (var detail in Details(reader, page))
                details.Add(new ExtendedDetail() { Item = detail.Item, PartNo = detail.PartNo, ValidFor = detail.ValidFor, Quantity = detail.Quantity, Unit = detail.Unit, Assembly = assembly });

            return details;
        }
    }
}