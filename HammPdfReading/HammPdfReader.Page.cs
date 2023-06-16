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
                var selectingReader = new PdfReader(_reader);
                selectingReader.SelectPages(new[] { pageIndex });
                concatenate.AddPages(selectingReader);
                selectingReader.Close();
            };
    }
}
