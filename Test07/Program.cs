﻿// See https://aka.ms/new-console-template for more information
using iTextSharp.text.pdf;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Csv;

Console.WriteLine("путь к документу:");
var pdfReader = new PdfReader(Console.ReadLine());
var reader = new HammPdfReader(pdfReader);
var details = reader.GetExtendedDetails(91);
var modules = reader.GetModule(91);

Console.WriteLine("путь к базе:");
var path = Console.ReadLine();

var builder = new HammPdfWriter(path);
builder.Build();
builder.Join(details.ToArray(), new[] { modules });