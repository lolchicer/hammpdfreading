using iTextSharp.text.pdf;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Csv;

// вывод
var output = string.Empty;

// распаршенные аргументы/нуллеваемость для вывода
string? pdfPath = null;
string databasePath = Path.GetDirectoryName(
    System.Reflection.Assembly.GetExecutingAssembly().Location) +
    "\\hamm.csv" ??
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

// число лишних аргументов/нуллеваемость для вывода
int excessiveArgsCount = 0;

// лямбда-выражения для парсинга аргументов
Action<string>[] validations = {
    (arg) => pdfPath = arg, 
    (arg) => databasePath = arg};

// парсинг аргументов
for (int i = 0; i < args.Length; i++)
    if (args[i] != null)
        validations[i](args[i]);
    else
        excessiveArgsCount++;

// запись pdf-файлов
if (pdfPath != null)
{
    var builder = new HammPdfWriter(databasePath);

    if (!File.Exists(databasePath))
        builder.Build();

    foreach (var file in Directory.GetFiles(pdfPath))
    {
        var pdfReader = new PdfReader(file);
        var reader = new HammPdfReader(pdfReader);

        await Task.Run(() => builder.Join(reader.ExtendedDetails(), reader.GetModules()));
        Console.WriteLine($"Содержимое {file} выведено в {databasePath}.\n");
    }
}

// сборка сообщения для вывода

if (pdfPath == null)
    output += "Не указан путь к PDF-файлам.\n";

if (excessiveArgsCount > 0)
    output += $"Лишних аргументов: {excessiveArgsCount}.\n";

// вывод
Console.Write(output);
