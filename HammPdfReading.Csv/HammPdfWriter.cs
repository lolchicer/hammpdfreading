﻿using CsvHelper;
using System.Globalization;
using System.Text;

namespace Infor.HammPdfReading.Csv
{
    public class HammPdfWriter
    {
        string _path;

        string Path => System.IO.Path.Combine(_path, "table.csv");

        public void Build()
        {
            using (FileStream stream = File.Create(Path)) ;
            JoinHeader();
        }

        public void Insert(ExtendedDetail detail)
        {
            using (StreamWriter writer = new StreamWriter(Path, true, Encoding.UTF8))
            {
                using (CsvWriter csvWriter = new CsvWriter(writer, new CultureInfo("ru-RU", false)))
                {
                    csvWriter.WriteRecord(detail);
                }
            }
        }

        public void Insert(ExtendedDetail[] details)
        {
            using (StreamWriter writer = new StreamWriter(Path, true, Encoding.UTF8))
            {
                using (CsvWriter csvWriter = new CsvWriter(writer, new CultureInfo("ru-RU", false)))
                {
                    csvWriter.WriteRecords(details);
                }
            }
        }

        void JoinHeader()
        {
            using (StreamWriter writer = new StreamWriter(Path, true, Encoding.UTF8))
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
                }
            }
        }

        public void Join(ExtendedDetail[] details, Module[] modules)
        {
            using (StreamWriter writer = new StreamWriter(Path, true, Encoding.UTF8))
            {
                using (CsvWriter csvWriter = new CsvWriter(writer, new CultureInfo("ru-RU", false)))
                {
                    foreach (var module in modules)
                    {
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
                        var imageFolderPath = System.IO.Path.Combine(_path, module.No);
                        Directory.CreateDirectory(imageFolderPath);
                        for (int i = 0; i < module.Images.Length; i++)
                            module.Images[i].Save(System.IO.Path.Combine(imageFolderPath, $"{i}.jpg"));
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