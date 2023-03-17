// See https://aka.ms/new-console-template for more information
using iTextSharp.text.pdf;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Sqlite;

Console.WriteLine("строка:");
Detail detail = (Detail)Console.ReadLine();

Console.WriteLine("путь к базе:");
var path = Console.ReadLine();

Builder.Build(path);
Builder.Insert(path, detail);
