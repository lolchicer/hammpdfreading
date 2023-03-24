using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace Infor.HammPdfReading
{
    public enum Unit
    {
        PC,
        M
    }

    public struct Detail
    {
        public double Item;
        public int PartNo;
        public ValueTuple<int, int> ValidFor;
        public double Quantity;
        public Unit Unit;
        public string Designation;

        public override string ToString()
        {
            string UnitToString(Unit unit)
            {
                return unit switch
                {
                    Unit.PC => "PC",
                    Unit.M => "M",
                    _ => "PC"
                };
            }

            return $"{Item} {PartNo} {ValidFor.Item1}-{ValidFor.Item2} {Quantity} {UnitToString(Unit)} {Designation}";
        }

        static ValueTuple<int, int> ToValidFor(string s)
        {
            string[] array = s.Split('-');
            return new ValueTuple<int, int>(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
        }

        static Unit ToUnitType(string s)
        {
            return s switch
            {
                "PC" => Unit.PC,
                "M" => Unit.M,
                _ => Unit.PC
            };
        }

        public static implicit operator Detail(string s)
        {
            var details = Reader.Fields(s);
            return new Detail()
            {
                Item = Convert.ToDouble(details[0].Replace('.', ',')),
                PartNo = Convert.ToInt32(details[1]),
                ValidFor = ToValidFor(details[2]),
                Quantity = Convert.ToDouble(details[3]),
                Unit = ToUnitType(details[4]),
                Designation = details[6]
            };
        }

        public static Detail FromFields(List<string> fields) => new Detail()
        {
            Item = Convert.ToDouble(fields[0].Replace('.', ',')),
            PartNo = Convert.ToInt32(fields[1]),
            ValidFor = ToValidFor(fields[2]),
            Quantity = Convert.ToDouble(fields[3]),
            Unit = ToUnitType(fields[4]),
            Designation = fields[6]
        };
    }

    public struct ExtendedDetail
    {
        public Detail Detail { get; set; }
        public int Assembly { get; set; }
    }

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

            var pageInfo = PageInfo(Regex.Match(text, "\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}(.|\n)*").Value);

            var details = new List<ExtendedDetail>();

            foreach (var detail in Details(reader, page))
                details.Add(new ExtendedDetail() { Detail = detail, Assembly = Convert.ToInt32(pageInfo[1]) });

            return details;
        }
    }
}