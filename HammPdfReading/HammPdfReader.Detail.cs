using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infor.HammPdfReading
{
    public partial class HammPdfReader
    {
        enum RowType
        {
            full,
            main,
            designation
        }

        static Detail[] GetDetails(string text)
        {
            var details = new List<Detail>();

            var textCut = Regex.Match(text, Regexes.Table).Value.Trim();
            var isFinished = true;

            RowType rowType;

            var designations = new List<string>();

            void Finish()
            {
                var i = details.Count - 1;
                var replacing = details[i];
                replacing.Designation += DesignationRussian(designations.ToArray());
                replacing.Designation = replacing.Designation.Trim();
                details.RemoveAt(i);
                details.Add(replacing);

                isFinished = true;
            }

            foreach (var line in textCut.Split('\n'))
            {
                var row = ContinuousMatch(line, Regexes.TableLineToArray());
                if (!row.Contains(string.Empty))
                    rowType = RowType.full;
                else
                {
                    row = ContinuousMatch(line, Regexes.MainToArray());
                    if (!row.Contains(string.Empty))
                        rowType = RowType.main;
                    else
                    {
                        row = ContinuousMatch(line, Regexes.DesignationsToArray());
                        rowType = RowType.designation;
                    }
                }

                switch (rowType)
                {
                    case RowType.full:
                        if (!isFinished) Finish();

                        details.Add(Detail.FromFields(row));
                        break;
                    case RowType.main:
                        if (!isFinished) Finish();

                        row = ContinuousMatch(line, Regexes.MainToArray());
                        var extendedRow = new string[row.Length + 2];
                        row.CopyTo(extendedRow, 0);
                        new[] { string.Empty, string.Empty }.CopyTo(extendedRow, 5);
                        details.Add(Detail.FromFields(extendedRow));

                        isFinished = false;
                        break;
                    case RowType.designation:
                        designations.Add(line);
                        break;
                }
            }

            return details.ToArray();
        }

        public Detail[] GetDetails(int page)
        {
            var strategy = new SimpleTextExtractionStrategy();

            var text = PdfTextExtractor.GetTextFromPage(_reader, page, strategy);
            var match = Regex.Match(text, Regexes.Table);

            return GetDetails(match.Value);
        }
    }
}
