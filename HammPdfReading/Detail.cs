using Infor.HammPdfReading;

namespace HammPdfReading
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
}
