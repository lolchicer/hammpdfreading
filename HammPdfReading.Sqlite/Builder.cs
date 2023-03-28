using iTextSharp.text.pdf;
using Microsoft.Data.Sqlite;

namespace Infor.HammPdfReading.Sqlite
{
    public class Builder
    {
        SqliteConnection _connection;

        static string BuildQuery() =>
            "CREATE TABLE details (" +
            "   assembly INTEGER NOT NULL," +
            "   item REAL NOT NULL," +
            "   part_no INTEGER NOT NULL," +
            "   valid_for_1 INTEGER NOT NULL," +
            "   valid_for_2 INTEGER NOT NULL," +
            "   quantity REAL NOT NULL," +
            "   unit INTEGER NOT NULL," +
            "   designation TEXT NOT NULL," +
            "   PRIMARY KEY (assembly, item, part_no)" +
            ")";

        static string InsertQueryValues(ExtendedDetail detail) =>
            $"({detail.Item.ToString().Replace(',', '.')}, " +
            $"{detail.PartNo}, " +
            $"{detail.ValidFor.Item1}, " +
            $"{detail.ValidFor.Item2}, " +
            $"{detail.Quantity.ToString().Replace(',', '.')}, " +
            $"{(int)detail.Unit}, " +
            $"\"{detail.Designation}\", " +
            $"{detail.Assembly})";

        static string[] InsertQueryValues(ExtendedDetail[] details) =>
            details.Select(detail => InsertQueryValues(detail)).ToArray();

        static string InsertQuery(ExtendedDetail detail) =>
            $"INSERT INTO details (item, part_no, valid_for_1, valid_for_2, quantity, unit, designation, assembly)" +
            $"VALUES {InsertQueryValues(detail)}";

        static string InsertQuery(ExtendedDetail[] details) =>
            $"INSERT INTO details (item, part_no, valid_for_1, valid_for_2, quantity, unit, designation, assembly)" +
            $"VALUES {string.Concat(from detail in details where Array.IndexOf(details, detail) < details.Length - 1 select InsertQueryValues(detail) + ", ")}" +
            $"{InsertQueryValues(details.Last())}";

        void QuerySend(string query)
        {
            _connection.Open();
            
            using (SqliteCommand command = _connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }

            _connection.Close();
        }

        public void Build() => QuerySend(BuildQuery());

        public void Insert(ExtendedDetail detail) => QuerySend(InsertQuery(detail));

        public void Insert(ExtendedDetail[] details) => QuerySend(InsertQuery(details));

        public Builder(string path)
        {
            _connection = new SqliteConnection($"DataSource={path}");
        }
    }
}
