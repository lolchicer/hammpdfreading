// See https://aka.ms/new-console-template for more information
using Infor.HammPdfReading;

var detail = Console.ReadLine();
foreach (var item in Reader.Fields(detail))
    Console.WriteLine(item);

Console.WriteLine(((Detail)detail).ToString());
