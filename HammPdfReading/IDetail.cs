namespace Infor.HammPdfReading
{
    internal interface IDetail
    {
        public double Item { get; set; }
        public string PartNo { get; set; }
        public ValueTuple<int, int> ValidFor { get; set; }
        public double Quantity { get; set; }
        public Unit Unit { get; set; }
        public string Designation { get; set; }

        public string ToString() => $"{Item} {PartNo} {ValidFor.Item1}-{ValidFor.Item2} {Quantity} {UnitToString(Unit)} {Designation}";

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

        static string UnitToString(Unit unit)
        {
            return unit switch
            {
                Unit.PC => "PC",
                Unit.M => "M",
                _ => "PC"
            };
        }
    }
}
