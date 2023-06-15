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
        public void GetPageTest()
        {
            var pdfReader = new PdfReader(Path.Combine(Environment.CurrentDirectory, "res/K024 (MR 130 Z).pdf"));
            var reader = new HammPdfReader(pdfReader);

            reader.GetPage(90);
        }

        [TestMethod]
        public void PageTest()
        {
            var pdfReader = new PdfReader(Path.Combine(Environment.CurrentDirectory, "res/K024 (MR 130 Z).pdf"));
            var reader = new HammPdfReader(pdfReader);

            var writer = new HammPdfWriter("output");
            writer.Build();
            writer.Join(reader.GetExtendedDetails(19), new[] { reader.GetModule(19) });
        }

        [TestMethod]
        public void PageItextsharpTest()
        {
            var pdfReader = new PdfReader(Path.Combine(Environment.CurrentDirectory, "res/K024 (MR 130 Z).pdf"));
            using (var stream = new FileStream("itextsharp pageshot.pdf", FileMode.Create))
            {
                var concatenate = new PdfConcatenate(stream);

                pdfReader.SelectPages(new[] { 18 });
                concatenate.AddPages(pdfReader);

                // pdfReader.Close();
                concatenate.Close();
            }
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