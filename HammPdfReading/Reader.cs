using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace Infor.HammPdfReading {
    public enum Unit {
        PC,
        M
    }

    public struct Detail {
        public double Item;
        public int PartNo;
        public ValueTuple<int, int> ValidFor;
        public int Quantity;
        public Unit Unit;
        public string Designation;

        public override string ToString() {
            string UnitToString(Unit unit) {
                return unit switch {
                    Unit.PC => "PC",
                    Unit.M => "M",
                    _ => "PC"
                };
            }

            return $"{Item} {PartNo} {ValidFor.Item1}-{ValidFor.Item2} {Quantity} {UnitToString(Unit)} {Designation}";
        }

        public static implicit operator Detail(string s) {
            ValueTuple<int, int> ToValidFor(string s) {
                string[] array = s.Split('-');
                return new ValueTuple<int, int>(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
            }

            Unit ToUnitType(string s) {
                return s switch {
                    "PC" => Unit.PC,
                    "M" => Unit.M,
                    _ => Unit.PC
                };
            }

            var details = Reader.Details(s);
            return new Detail() {
                Item = Convert.ToDouble(details[0]),
                PartNo = Convert.ToInt32(details[1]),
                ValidFor = ToValidFor(details[2]),
                Quantity = Convert.ToInt32(details[3]),
                Unit = ToUnitType(details[4]),
                Designation = details[6]
            };
        }
    }

    public static class Reader {
        public static List<string> Details(string line) {
            var details = new List<string>();

            var lineCut = line;
            var regexes = new[] { "[0-9]+(\\.[0-9]{2})?", "[0-9]+", "[0-9]{1,4}-[0-9]{1,4}", "[0-9]+", "[A-Z]{1,2}", "[^А-Я]+", ".+" };
            string detail;
            foreach (var regex in regexes) {
                detail = Regex.Match(lineCut, regex).Value;
                detail = detail.Trim();
                details.Add(detail);
                lineCut = lineCut.Trim();
                lineCut = lineCut.Remove(0, detail.Length);
            };

            return details;
        }

        public static List<string> Details(PdfReader reader, int page) {
            var details = new List<string>();
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
            var pars = text.Split(new[] { "Ед.", "Benennung" }, StringSplitOptions.None);

            foreach (var line in pars[2].Split("\n"))
                details.AddRange(Details(line));

            return details;
        }
    }
}