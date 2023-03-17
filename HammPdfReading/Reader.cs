using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace Infor.HammPdfReading {
    public enum UnitType {
        PC,
        M
    }

    public struct Detail {
        public double Item;
        public int PartNo;
        public ValueTuple<int, int> ValidFor;
        public int Quantity;
        public UnitType Type;
        public string Designation;

        public override string ToString() {
            string UnitTypeToString(UnitType type) {
                return type switch {
                    UnitType.PC => "PC",
                    UnitType.M => "M",
                    _ => "PC"
                };
            }

            return $"{Item} {PartNo} {ValidFor.Item1}-{ValidFor.Item2} {Quantity} {UnitTypeToString(Type)} {Designation}";
        }

        public static implicit operator Detail(string s) {
            ValueTuple<int, int> ToValidFor(string s) {
                string[] array = s.Split('-');
                return new ValueTuple<int, int>(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
            }

            UnitType ToUnitType(string s) {
                return s switch {
                    "PC" => UnitType.PC,
                    "M" => UnitType.M,
                    _ => UnitType.PC
                };
            }

            var details = Reader.Details(s);
            return new Detail() {
                Item = Convert.ToDouble(details[0]),
                PartNo = Convert.ToInt32(details[1]),
                ValidFor = ToValidFor(details[2]),
                Quantity = Convert.ToInt32(details[3]),
                Type = ToUnitType(details[4]),
                Designation = details[6]
            };
        }
    }

    public static class Reader {
        public static List<string> Details(PdfPage page) {
            var details = new List<string>();

            foreach (var key in page.Keys) {
                var object_ = (PdfDictionary)page.Get(key);
                details.Add(object_.Get(PdfName.TEXT).ToString());
            }

            return details;
        }

        public static List<string> Details(PdfDictionary dictionary) {
            var details = new List<string>();
            var objects = new List<PdfObject>();

            foreach (var key in dictionary.Keys) {
                var object_ = dictionary.Get(key);
                objects.Add(object_);
                // details.Add(object_.Get(PdfName.TEXT).ToString());
            }

            return details;
        }

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