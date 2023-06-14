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
        public Func<PdfCopy, PdfImportedPage> GetPage(int pageIndex) =>
            (copy) => copy.GetImportedPage(_reader, pageIndex);
    }
}
