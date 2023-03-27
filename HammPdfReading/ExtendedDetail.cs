namespace Infor.HammPdfReading
{
    public struct ExtendedDetail : IDetail
    {
        public double Item { get; set; }
        public int PartNo { get; set; }
        public ValueTuple<int, int> ValidFor { get; set; }
        public double Quantity { get; set; }
        public Unit Unit { get; set; }
        public string Designation { get; set; }
        public int Assembly { get; set; }

        public override string ToString() => ((IDetail)this).ToString() + Assembly;
    }
}
