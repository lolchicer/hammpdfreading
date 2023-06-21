using Microsoft.AspNetCore.Mvc;
using iTextSharp.text.pdf;

namespace Infor.HammPdfReading.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExtendedDetailController : Controller
    {
        [HttpGet("{path}")]
        public IEnumerable<ExtendedDetail> ExtendedDetails(string path)
        {
            var pdfReader = new PdfReader(path);
            var reader = new HammPdfReader(pdfReader); 

            return reader.GetExtendedDetails(40, 4);
        }
    }
}
