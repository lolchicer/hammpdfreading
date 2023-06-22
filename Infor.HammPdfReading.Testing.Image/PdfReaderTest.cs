using iTextSharp.text.pdf;

namespace Infor.HammPdfReading.Testing
{
    [TestClass]
    public class PdfReaderTest
    {
        [TestMethod]
        public void StreamMethod()
        {
            using (var stream = new FileStream("res/K024 (MR 130 Z).pdf", FileMode.Open))
            {
                var reader = new PdfReader(stream);
            }
        }

        [TestMethod]
        public void FormToMemoryMethod()
        {
            using (var fileStream = new FileStream("res/K024 (MR 130 Z).pdf", FileMode.Open))
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    var reader = new PdfReader(memoryStream);
                }
            }
        }
    }
}
