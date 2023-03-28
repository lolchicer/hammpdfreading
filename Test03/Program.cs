// See https://aka.ms/new-console-template for more information
using iTextSharp.text.pdf;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Sqlite;

Console.WriteLine("путь к документу:");
var reader = new PdfReader(Console.ReadLine());
var details = Reader.ExtendedDetails(reader, 329);

Console.WriteLine("путь к базе:");
var path = Console.ReadLine();

var builder = new Builder(path);
builder.Build();
builder.Insert(details.ToArray());
