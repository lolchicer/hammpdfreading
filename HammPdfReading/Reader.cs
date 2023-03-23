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
            var details = Reader.Details(s);
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

    public static class Reader
    {
        static string[] _regexes = new[] { "[0-9]+(\\.[0-9]{2})?", "[0-9]+", "[0-9]{1,4}-[0-9]{1,4}", "[0-9]+", "[A-Z]{1,2}", "[^А-я]+", ".+" };

        static string Field(string line, int i)
        {
            string field;
            field = Regex.Match(line, _regexes[i]).Value;
            field = field.Trim();
            return field;
        }

        // _regexes[6] почему-то не считает первую букву "С" в "СФЕРО-ЦИЛИНДРИЧЕСКОЙ" на странице 215
        public static List<string> Details(string line)
        {
            var details = new List<string>();

            var lineCut = line;
            string detail;
            foreach (var regex in _regexes)
            {
                detail = Regex.Match(lineCut, regex).Value;
                detail = detail.Trim();
                details.Add(detail);
                lineCut = lineCut.Trim();
                lineCut = lineCut.Remove(0, detail.Length);
            };

            return details;
        }

        public static List<string> Designations(string line) =>
            new List<string>()
            {
                Field(line, 5),
                Field(line, 6)
            };

        public static List<Detail> Details(PdfReader reader, int page)
        {
            var details = new List<Detail>();
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
            var match = Regex.Match(text,
                "(?<=Ед\\.\n)" +
                "(.|\n)*" +
                "(?=\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2})");

            foreach (var line in match.Value.Split('\n'))
            {
                var row = Details(line);
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
    }
}