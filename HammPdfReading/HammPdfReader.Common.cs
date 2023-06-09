using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace Infor.HammPdfReading
{
    public partial class HammPdfReader
    {
        static string[] ContinuousMatch(string text, string[] regexes)
        {
            var value = new List<string>();

            var textCut = text.Trim();
            foreach (var regex in regexes)
            {
                var match = Regex.Match(textCut, regex);
                var item = match.Value;
                item = item.Trim();
                value.Add(item);
                textCut = textCut.Remove(0, match.Index + match.Length);
                textCut = textCut.Trim();
            }

            return value.ToArray();
        }

        static bool IsTablePage(string text) => Regex.IsMatch(text, Regexes.Table);

        PdfReader _reader;

        public HammPdfReader(PdfReader reader)
        {
            _reader = reader;
        }
    }
}