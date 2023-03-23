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
        static string[] _regexes = new[] { "[0-9]+(\\.[0-9]{2})?", "[0-9]+", "[0-9]{1,4}-[0-9]{1,4}", "[0-9]+", "[A-Z]{1,2}", "[^А-Я]+", ".+" };

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

        public static List<Detail> Details(PdfReader reader, int page)
        {
            var details = new List<Detail>();
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
            var match = Regex.Match(text,
                "(?<=Ед\\.\n)" +
                "(.|\n)*" +
                "(?=\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2})");

            var matrix = new List<List<string>>();

            foreach (var line in match.Value.Split('\n'))
                matrix.Add(Details(line));

            var buffMatrix = new List<List<string>>();

            foreach (var row in matrix)
            {
                // поиск значений для каждого поля
                buffMatrix.Add(row);

                var buffContains = new List<bool>(new bool[7]);
                for (int i = 0; i < row.Count; i++)
                    if (row[i] != null)
                        buffContains[i] = true;

                // после успешного поиска значений для каждого поля
                if (!buffContains.Contains(false))
                {
                    // проверка на симметрию
                    var buffSymmetric = true;

                    var count = buffMatrix.Count;
                    if (count % 2 != 0)
                    {
                        count /= 2;
                        count += 1;
                    }

                    for (int j = 0; j < count / 2; j++)
                        for (int i = 0; i < 7; i++)
                            if (!(buffMatrix[j][i] != null && buffMatrix[buffMatrix.Count - 1 - j][i] != null))
                                buffSymmetric = false;

                    // соединение полей
                    var buffSingle = new List<string>();

                    if (buffSymmetric)
                        for (int i = 0; i < 7; i++)
                        {
                            buffSingle.Add(string.Empty);
                            foreach (var buffRow in buffMatrix)
                                buffSingle[i] += buffRow[i];
                        }

                    // конец
                    details.Add(Detail.FromFields(buffSingle));
                }
            }

            return details;
        }
    }
}