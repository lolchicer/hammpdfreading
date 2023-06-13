using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Infor.HammPdfReading
{
    public partial class HammPdfReader
    {
        public byte[][] GetImages(int pageIndex)
        {
            var page = _reader.GetPageN(pageIndex);
            var resources = PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES)) as PdfDictionary;
            var xobj = PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT)) as PdfDictionary;
            var keys = xobj.Keys;

            var objs = from key in keys select xobj.Get(key);

            var xrefIndices = from obj in objs select (obj as PRIndirectReference).Number;
            var pdfStreams = from xrefIndex in xrefIndices select _reader.GetPdfObject(xrefIndex) as PRStream;
            var datas = from pdfStream in pdfStreams select PdfReader.GetStreamBytesRaw(pdfStream);

            return datas.ToArray();
        }

        /// <summary>
        /// получает изображения со страниц в указанном диапазоне. внутри функции значения параметров увеличиваются на один.
        /// </summary>
        /// <param name="startIndex">первая страница</param>
        /// <param name="count">число страниц</param>
        /// <returns></returns>
        public byte[][][] GetImages(int startIndex, int count) =>
            (from index in Enumerable.Range(startIndex + 1, count + 1) select GetImages(index)).ToArray();

        public byte[][][] GetImages() =>
            (from index in Enumerable.Range(0, _reader.NumberOfPages) select GetImages(index)).ToArray();
    }
}
