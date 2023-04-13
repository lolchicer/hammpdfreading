using CsvHelper;
using System.Globalization;
using System.Text;

namespace Infor.HammPdfReading.Csv
{
    public class Builder
    {
        string _path;

        public void Build()
        {
            using (FileStream stream = File.Create(_path)) ;
        }

        public void Insert(ExtendedDetail detail)
        {
            using (StreamWriter writer = new StreamWriter(_path, true, Encoding.UTF8))
            {
                using (CsvWriter csvWriter = new CsvWriter(writer, new CultureInfo("ru-RU", false)))
                {
                    csvWriter.WriteRecord(detail);
                }
            }
        }

        public void Insert(ExtendedDetail[] details)
        {
            using (StreamWriter writer = new StreamWriter(_path, true, Encoding.UTF8))
            {
                using (CsvWriter csvWriter = new CsvWriter(writer, new CultureInfo("ru-RU", false)))
                {
                    csvWriter.WriteRecords(details);
                }
            }
        }

        public Builder (string path)
        {
            _path = path;
        }
    }
}