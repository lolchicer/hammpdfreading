namespace Infor.HammPdfReading
{
    public struct ExtendedDetail : IDetail
    {
        public double Item { get; set; }
        public string PartNo { get; set; }
        public ValueTuple<int, int> ValidFor { get; set; }
        public double Quantity { get; set; }
        public Unit Unit { get; set; }
        public string Designation { get; set; }
        public int Assembly { get; set; }

        public override string ToString() => $"{Item} {PartNo} {ValidFor.Item1}-{ValidFor.Item2} {Quantity} {IDetail.UnitToString(Unit)} {Designation} {Assembly}";
    }
}
