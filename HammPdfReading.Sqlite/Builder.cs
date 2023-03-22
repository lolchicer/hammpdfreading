using Microsoft.Data.Sqlite;
using Infor.HammPdfReading;

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
                "   designation TEXT NOT NULL" +
                ")";
            command.ExecuteNonQuery();
        }

        public static void Insert(string path, Detail detail) {
            var connection = new SqliteConnection($"DataSource={path}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"" +
                $"INSERT INTO details (item, part_no, valid_for_1, valid_for_2, quantity, unit, designation)" +
                $"VALUES ({detail.Item.ToString().Replace(',', '.')}, " +
                $"{detail.PartNo}, " +
                $"{detail.ValidFor.Item1}, " +
                $"{detail.ValidFor.Item2}, " +
                $"{detail.Quantity.ToString().Replace(',', '.')}, " +
                $"{(int)detail.Unit}, " +
                $"\"{detail.Designation}\")";
            command.ExecuteNonQuery();
        }

        public static void Insert(string path, Detail[] details) {
            foreach (var detail in details)
                Insert(path, detail);
        }
    }
}
