using Infor.HammPdfReading;

namespace Infor.HammPdfReading
{
    public struct Detail : IDetail
    {
        public double Item { get; set; }
        public int PartNo { get; set; }
        public ValueTuple<int, int> ValidFor { get; set; }
        public double Quantity { get; set; }
        public Unit Unit { get; set; }
        public string Designation { get; set; }

        public override string ToString() => $"{Item} {PartNo} {ValidFor.Item1}-{ValidFor.Item2} {Quantity} {IDetail.UnitToString(Unit)} {Designation}";

        public static implicit operator Detail(string s)
        {
            var details = Reader.Fields(s);
            return new Detail()
            {
                Item = Convert.ToDouble(details[0].Replace('.', ',')),
                PartNo = Convert.ToInt32(details[1]),
                ValidFor = IDetail.ToValidFor(details[2]),
                Quantity = Convert.ToDouble(details[3]),
                Unit = IDetail.ToUnitType(details[4]),
                Designation = details[6]
            };
        }

        public static Detail FromFields(List<string> fields) => new Detail()
        {
            Item = Convert.ToDouble(fields[0].Replace('.', ',')),
            PartNo = Convert.ToInt32(fields[1]),
            ValidFor = IDetail.ToValidFor(fields[2]),
            Quantity = Convert.ToDouble(fields[3]),
            Unit = IDetail.ToUnitType(fields[4]),
            Designation = fields[6]
        };
    }
}
