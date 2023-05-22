namespace Infor.HammPdfReading
{
    public struct ExtendedDetail : IDetail
    {
        Detail _detail;
        
        public Detail Detail { 
            get { return _detail; } 
            set { _detail = value; } }

        public double Item {
            get { return _detail.Item; }
            set { _detail.Item = value; } }
        public int PartNo {
            get { return _detail.PartNo; }
            set { _detail.PartNo = value; } }
        public ValueTuple<int, int> ValidFor {
            get { return _detail.ValidFor; }
            set { _detail.ValidFor = value; } }
        public double Quantity {
            get { return _detail.Quantity; }
            set { _detail.Quantity = value; } }
        public Unit Unit {
            get { return _detail.Unit; }
            set { _detail.Unit = value; } }
        public string Designation {
            get { return _detail.Designation; }
            set { _detail.Designation = value; }
        }

        public int Assembly { get; set; }

        public override string ToString() => $"{Item} {PartNo} {ValidFor.Item1}-{ValidFor.Item2} {Quantity} {IDetail.UnitToString(Unit)} {Designation} {Assembly}";
    }
}
