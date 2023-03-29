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

        public override string ToString() => ((IDetail)this).ToString();

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
