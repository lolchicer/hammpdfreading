using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infor.HammPdfReading
{
    public partial class HammPdfReader
    {
        public Action<PdfConcatenate> GetPage(int pageIndex) =>
            (concatenate) =>
            {
                _reader.SelectPages(new[] { pageIndex });
                concatenate.AddPages(_reader);
            };
    }
}
