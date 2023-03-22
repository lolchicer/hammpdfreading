// See https://aka.ms/new-console-template for more information
using iTextSharp.text.pdf;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Sqlite;

Console.WriteLine("путь к документу:");
var reader = new PdfReader(Console.ReadLine());
var details = Reader.Details(reader, 53);

Console.WriteLine("путь к базе:");
var path = Console.ReadLine();

Builder.Build(path);
foreach (var item in details)
    Builder.Insert(path, item);
