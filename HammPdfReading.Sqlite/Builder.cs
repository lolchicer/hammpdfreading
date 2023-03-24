using Microsoft.Data.Sqlite;

namespace Infor.HammPdfReading.Sqlite {
    public class Builder {
        public static void Build(string path) {
            var connection = new SqliteConnection($"DataSource={path}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "" +
                "CREATE TABLE details (" +
                "   item REAL NOT NULL," +
                "   part_no INTEGER PRIMARY KEY," +
                "   valid_for_1 INTEGER NOT NULL," +
                "   valid_for_2 INTEGER NOT NULL," +
                "   quantity REAL NOT NULL," +
                "   unit INTEGER NOT NULL," +
                "   designation TEXT NOT NULL," +
                "   assembly INT NOT NULL" +
                ")";
            command.ExecuteNonQuery();
        }

        public static void Insert(string path, ExtendedDetail detail) {
            var connection = new SqliteConnection($"DataSource={path}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"" +
                $"INSERT INTO details (item, part_no, valid_for_1, valid_for_2, quantity, unit, designation, assembly)" +
                $"VALUES ({detail.Detail.Item.ToString().Replace(',', '.')}, " +
                $"{detail.Detail.PartNo}, " +
                $"{detail.Detail.ValidFor.Item1}, " +
                $"{detail.Detail.ValidFor.Item2}, " +
                $"{detail.Detail.Quantity.ToString().Replace(',', '.')}, " +
                $"{(int)detail.Detail.Unit}, " +
                $"\"{detail.Detail.Designation}\"," +
                $"{detail.Assembly})";
            command.ExecuteNonQuery();
        }

        public static void Insert(string path, ExtendedDetail[] details) {
            foreach (var detail in details)
                Insert(path, detail);
        }
    }
}
