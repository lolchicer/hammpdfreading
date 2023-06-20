using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace Infor.HammPdfReading
{
    public partial class HammPdfReader
    {
        /// <summary>
        /// выводит текст в Module. при этом картинки игнорируются.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Module GetModule(string text)
        {
            var pageInfo = ContinuousMatch(
                Regex.Match(text, "\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}(.|\n)*").Value,
                Regexes.PageInfoToArray());

            return new Module() { No = pageInfo[1], Assembly = pageInfo[0], Series = pageInfo[5], Designation = DesignationRussianLines(pageInfo[3]) };
        }

        public Module GetModule(int page)
        {
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(_reader, page, strategy);

            var module = GetModule(text);
            module.Images = GetImages(page - 1);
            module.ImagePageWrite = GetPage(page - 1);

            return module;
        }

        public Module[] GetModules(int page, int count)
        {
            var modules = new List<Module>();
            for (int i = 0; i < count; i++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(_reader, page + i, strategy);
                if (IsTablePage(text))
                    modules.Add(GetModule(page + i));
            }
            return modules.ToArray();
        }

        public Module[] GetModules()
        {
            return GetModules(1, _reader.NumberOfPages);
        }
    }
}
