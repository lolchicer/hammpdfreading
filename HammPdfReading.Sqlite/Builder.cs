using Microsoft.Data.Sqlite;
using Infor.HammPdfReading;

namespace Infor.HammPdfReading.Sqlite {
    public static class Builder {
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
                "   quantity INTEGER NOT NULL," +
                "   unit INTEGER NOT NULL," +
                "   designation TEXT NOT NULL" +
                ")";
            command.ExecuteNonQuery();
        }
    }
}
