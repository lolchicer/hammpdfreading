namespace Infor.HammPdfReading
{
    public struct ExtendedDetail : IDetail
    {
        IDetail _iDetail;

        public double Item {
            get { return _iDetail.Item; }
            set { _iDetail.Item = value; } }
        public int PartNo {
            get { return _iDetail.PartNo; }
            set { _iDetail.PartNo = value; } }
        public ValueTuple<int, int> ValidFor {
            get { return _iDetail.ValidFor; }
            set { _iDetail.ValidFor = value; } }
        public double Quantity {
            get { return _iDetail.Quantity; }
            set { _iDetail.Quantity = value; } }
        public Unit Unit {
            get { return _iDetail.Unit; }
            set { _iDetail.Unit = value; } }
        public string Designation {
            get { return _iDetail.Designation; }
            set { _iDetail.Designation = value; }
        }
        public int Assembly { get; set; }

        public override string ToString() => $"{Item} {PartNo} {ValidFor.Item1}-{ValidFor.Item2} {Quantity} {IDetail.UnitToString(Unit)} {Designation} {Assembly}";

        public ExtendedDetail(Detail detail, int assembly)
        {
            _iDetail = detail;
            Assembly = assembly;
        }
    }
}
