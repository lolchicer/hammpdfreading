using iTextSharp.text.pdf;

namespace Infor.HammPdfReading
{
    public struct Module
    {
        public string No { get; set; }
        public string Assembly { get; set; }
        public string Series { get; set; }
        public string Designation { get; set; }
        public byte[][]? Images { get; set; }
        public Func<PdfCopy, PdfImportedPage> ImagePageConstruct { get; set; }
    }
}
