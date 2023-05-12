// See https://aka.ms/new-console-template for more information
using iTextSharp;
using iTextSharp.text.pdf;
using Infor.HammPdfReading;

Console.WriteLine("путь к файлу:");
var path = Console.ReadLine();

var reader = new HammPdfReader(new PdfReader(path));
var details = reader.Details(37);

Console.WriteLine("поле:");
var field = Convert.ToInt32(Console.ReadLine());

Console.WriteLine(details[field]);
