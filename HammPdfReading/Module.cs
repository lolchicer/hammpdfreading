using System.Drawing;

namespace Infor.HammPdfReading
{
    public struct Module
    {
        public string No { get; set; }
        public string Assembly { get; set; }
        public string Series { get; set; }
        public string Designation { get; set; }
        public Image[]? Images { get; set; }
    }
}
