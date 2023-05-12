// See https://aka.ms/new-console-template for more information
using iTextSharp.text.pdf;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Csv;

Console.WriteLine("путь к документу:");
var reader = new HammPdfReader(new PdfReader(Console.ReadLine()));
var details = reader.ExtendedDetails(45, 10);

Console.WriteLine("путь к базе:");
var path = Console.ReadLine();

var builder = new HammPdfWriter(path);
builder.Build();
builder.Insert(details.ToArray());