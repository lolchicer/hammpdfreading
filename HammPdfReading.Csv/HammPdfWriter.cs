using CsvHelper;
using System.Globalization;
using System.Text;

namespace Infor.HammPdfReading.Csv
{
    public class HammPdfWriter
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

        public void Join(ExtendedDetail[] details, Module[] modules)
        {
            using (StreamWriter writer = new StreamWriter(_path, true, Encoding.UTF8))
            {
                using (CsvWriter csvWriter = new CsvWriter(writer, new CultureInfo("ru-RU", false)))
                {
                    foreach (var header in new string[]
                    {
                        "Позиция",
                        "№",
                        "Действует от",
                        "Действует до",
                        "Количество",
                        "Ед.",
                        "Наименование",
                        "Модуль",
                        "Группа",
                        "Конструктивный ряд",
                        "Наименование"
                    })
                        csvWriter.WriteField(header);
                    csvWriter.NextRecord();
                    foreach (var module in modules)
                        foreach (var detail in details)
                            if (detail.Assembly == module.No)
                            {
                                foreach (var field in new string[]
                                {
                                    detail.Item.ToString(),
                                    detail.PartNo.ToString(),
                                    detail.ValidFor.Item1.ToString(),
                                    detail.ValidFor.Item2.ToString(),
                                    detail.Quantity.ToString(),
                                    detail.Unit.ToString(),
                                    detail.Designation,
                                    detail.Assembly.ToString(),
                                    module.Assembly.ToString(),
                                    module.Series,
                                    module.Designation,
                                })
                                    csvWriter.WriteField(field);
                                csvWriter.NextRecord();
                            }
                }
            }
        }

        public HammPdfWriter (string path)
        {
            _path = path;
        }
    }
}