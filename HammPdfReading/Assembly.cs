namespace Infor.HammPdfReading
{
    public struct Assembly
    {
        public int No { get; set; }
        public string Designation { get; set; }

        public string ToString() => $"{No} {Designation}";
    }
}
