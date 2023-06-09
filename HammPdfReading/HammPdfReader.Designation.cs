using System.Text.RegularExpressions;

namespace Infor.HammPdfReading
{
    public partial class HammPdfReader
    {
        static string DesignationRussian(string text)
        {
            var designations = ContinuousMatch(text, Regexes.DesignationsToArray());
            if (designations[1] == string.Empty)
            {
                // гигантское количество функций, разворачивающих строку
                var reversed = text.ToCharArray();
                Array.Reverse(reversed);
                var reversedMatched = Regex.Match(new string(reversed), "((.|\\n)*)(.|\\n)*\\1");
                var matched = reversedMatched.Groups[0].Value.ToCharArray();
                Array.Reverse(matched);
                return new string(matched) ?? string.Empty;
            }
            else
                return designations[1];
        }

        static string DesignationRussian(string[] lines)
        {
            var text = string.Join(" ", lines);
            return DesignationRussian(text.Trim());
        }

        static string DesignationRussianLines(string text)
        {
            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Trim();
            return DesignationRussian(string.Concat(lines));
        }

        static string DesignationRussianLines(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Trim();
            return DesignationRussian(string.Concat(lines));
        }
    }
}
