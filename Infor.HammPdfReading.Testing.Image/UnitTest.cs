using iTextSharp.text.pdf;
using Infor.HammPdfReading.Csv;

namespace Infor.HammPdfReading.Testing.Image
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pdfReader = new PdfReader(Path.Combine(Environment.CurrentDirectory, "res/K024 (MR 130 Z).pdf"));
            var reader = new HammPdfReader(pdfReader);

            //var writer = new HammPdfWriter("test.csv");
            reader.GetImages(94);
        }
    }
}