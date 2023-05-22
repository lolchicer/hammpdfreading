// See https://aka.ms/new-console-template for more information
using iTextSharp.text.pdf;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Csv;
using iTextSharp.text.pdf.parser;

var pdfReader = new PdfReader(args[0]);
var strategy = new SimpleTextExtractionStrategy();
var text = PdfTextExtractor.GetTextFromPage(pdfReader, 91, strategy);

var context = new Context<List<Detail>>() { Result = new List<Detail>(), Text = text };

var expression = new DetailTableExprssion();

expression.Interpret(context);

var writer = new HammPdfWriter("bebra.csv");
writer.Build();
writer.Insert((from detail in context.Result select new ExtendedDetail(detail, 123)).ToArray());