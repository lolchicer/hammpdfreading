﻿// See https://aka.ms/new-console-template for more information
using iTextSharp.text.pdf;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Sqlite;

Console.WriteLine("путь к документу:");
var reader = new HammPdfReader(new PdfReader(Console.ReadLine()));
var details = reader.GetExtendedDetails(45);

Console.WriteLine("путь к базе:");
var path = Console.ReadLine();

var builder = new HammPdfWriter(path);
builder.Build();
builder.Insert(details.ToArray());
