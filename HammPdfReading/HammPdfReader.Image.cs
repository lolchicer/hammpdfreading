using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Infor.HammPdfReading
{
    public partial class HammPdfReader
    {
        public System.Drawing.Image[] GetImages(int pageIndex)
        {
            var page = _reader.GetPageN(pageIndex);
            var resources = PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES)) as PdfDictionary;
            var xobj = PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT)) as PdfDictionary;
            var keys = xobj.Keys;
            var obj = xobj.Get(keys.ElementAt(0));

            var xrefIndex = (obj as PRIndirectReference).Number;
            var pdfStream = _reader.GetPdfObject(xrefIndex) as PRStream;
            var data = PdfReader.GetStreamBytesRaw(pdfStream);

            return (
                from key in keys select 
                System.Drawing.Image.FromStream(
                    new MemoryStream(
                        ((_reader.GetPdfObject(
                            (xobj.Get(key) as PRIndirectReference)
                            .Number) as PRStream)
                            .GetBytes()))))
                            .ToArray();
        }

        /// <summary>
        /// получает изображения со страниц в указанном диапазоне. внутри функции значения параметров увеличиваются на один.
        /// </summary>
        /// <param name="startIndex">первая страница</param>
        /// <param name="count">число страниц</param>
        /// <returns></returns>
        public System.Drawing.Image[][] GetImages(int startIndex, int count) =>
            (from index in Enumerable.Range(startIndex + 1, count + 1) select GetImages(index)).ToArray();

        public System.Drawing.Image[][] GetImages() =>
            (from index in Enumerable.Range(0, _reader.NumberOfPages) select GetImages(index)).ToArray();
    }
}
