using Microsoft.AspNetCore.Mvc;
using iTextSharp.text.pdf;

namespace Infor.HammPdfReading.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExtendedDetailController : Controller
    {
        [HttpPost]
        public IEnumerable<ExtendedDetail> ExtendedDetails(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;

                var pdfReader = new PdfReader(stream);
                var reader = new HammPdfReader(pdfReader);

                return reader.GetExtendedDetails();
            }
        }
    }
}
