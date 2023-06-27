using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;

namespace Infor.HammPdfReading.WebApi
{
    [PrimaryKey(nameof(Series), nameof(No))]
    public class WebModule
    {
        public string? Series { get; set; }
        public string? No { get; set; }
        public string? Assembly { get; set; }
        public string? Designation { get; set; }
        public byte[]? SchemePage { get; set; }

        public List<WebDetail> Details { get; set; } = new();

        public WebModule() { }

        public WebModule(Module module, IEnumerable<WebDetail> details)
        {
            No = module.No;
            Series = module.Series;
            Assembly = module.Assembly;
            Designation = module.Designation;

            using (var stream = new MemoryStream())
            {
                var concatenate = new PdfConcatenate(stream);
                module.ImagePageWrite(concatenate);

                SchemePage = new byte[stream.Length];

                stream.Position = 0;
                stream.Read(SchemePage);

                concatenate.Close();
            }

            Details.AddRange(details);
        }
    }
}
