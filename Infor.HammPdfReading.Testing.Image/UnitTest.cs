using iTextSharp.text.pdf;
using Infor.HammPdfReading.Csv;

namespace Infor.HammPdfReading.Testing.Image
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void GetImagesTest()
        {
            var pdfReader = new PdfReader(Path.Combine(Environment.CurrentDirectory, "res/K024 (MR 130 Z).pdf"));
            var reader = new HammPdfReader(pdfReader);

            reader.GetImages(90);
        }

        [TestMethod]
        public void FullTest()
        {
            var pdfReader = new PdfReader(Path.Combine(Environment.CurrentDirectory, "res/K024 (MR 130 Z).pdf"));
            var reader = new HammPdfReader(pdfReader);

            var writer = new HammPdfWriter("output");
            writer.Build();
            writer.Join(reader.GetExtendedDetails(), reader.GetModules());
        }
    }
}