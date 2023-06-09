namespace Infor.HammPdfReading
{
    public partial class HammPdfReader
    {
        public static class Regexes
        {
            public const string Table =
                "(?<=Ед\\.\n)" +
                "(.|\n)*" +
                "(?=\n[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2})";

            public const string Item = "[0-9]+(\\.[0-9]{2})?";
            public const string PartNo = "(|F|M)[0-9]+";
            public const string ValidFor = "[0-9]{1,4}-[0-9]{1,4}";
            public const string Quantity = "[0-9]+";
            public const string Unit = "[A-Z]{1,2}";
            public const string DesignationSpace = "[^А-я]*";
            public const string DesignationRussian = ".+";

            public const string Assembly = "[0-9]{2}\\.[0-9]{2}\\.[0-9]{2} / [0-9]{2}";
            public const string Date = "[0-9]{2}\\.[0-9]{2}\\.[0-9]{4}";
            public const string AssemblyDesignations = "(.|\n)*(?=Benennung:)";
            public const string PageInfo5 = "ряд:";
            public const string Series = ".*";

            public static string[] TableLineToArray() => new[] {
                Item,
                PartNo,
                ValidFor,
                Quantity,
                Unit,
                DesignationSpace,
                DesignationRussian
            };

            public static string[] MainToArray() => new[]
            {
                Item,
                PartNo,
                ValidFor,
                Quantity,
                Unit
            };

            public static string[] DesignationsToArray() => new[]
            {
                DesignationSpace,
                DesignationRussian
            };

            public static string[] PageInfoToArray() => new[]
            {
                Assembly,
                PartNo,
                Date,
                AssemblyDesignations,
                PageInfo5,
                Series
            };
        }
    }
}
