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
                SchemePage = new byte[stream.Length];

                var concatenate = new PdfConcatenate(stream);
                module.ImagePageWrite(concatenate);

                stream.Position = 0;
                stream.Write(SchemePage);

                concatenate.Close();
            }

            Details.AddRange(details);
        }
    }
}
