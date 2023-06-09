using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace Infor.HammPdfReading
{
    public partial class HammPdfReader
    {
        static ExtendedDetail[] GetExtendedDetails(string text)
        {
            var details = new List<ExtendedDetail>();

            var pageInfo = ContinuousMatch(
                Regex.Match(text, "\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}(.|\n)*").Value,
                Regexes.PageInfoToArray());
            var assembly = pageInfo[1];

            foreach (var detail in GetDetails(text))
                details.Add(new ExtendedDetail()
                {
                    Item = detail.Item,
                    PartNo = detail.PartNo,
                    ValidFor = detail.ValidFor,
                    Quantity = detail.Quantity,
                    Unit = detail.Unit,
                    Designation = detail.Designation,
                    Assembly = assembly
                });

            return details.ToArray();
        }

        public ExtendedDetail[] GetExtendedDetails(int page)
        {
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(_reader, page, strategy);

            return GetExtendedDetails(text);
        }

        public ExtendedDetail[] GetExtendedDetails(int page, int count)
        {
            var details = new List<ExtendedDetail>();
            for (int i = 0; i < count; i++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(_reader, page + i, strategy);
                if (IsTablePage(text))
                    details.AddRange(GetExtendedDetails(text));
            }
            return details.ToArray();
        }

        public ExtendedDetail[] GetExtendedDetails() => GetExtendedDetails(1, _reader.NumberOfPages);
    }
}
