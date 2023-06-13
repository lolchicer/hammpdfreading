using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp.text.pdf;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Csv;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace Infor.HammPdfReading.Gui
{
    /// <summary>
    /// Логика взаимодействия для FormPage.xaml
    /// </summary>
    public partial class FormPage : Page
    {
        ILogger<FormPage> _logger;

        public FormPage()
        {
            InitializeComponent();
        }

        bool ConfirmButtonIsEnabled() =>
            (from driveInfo
             in DriveInfo.GetDrives()
             select Regex.Match(OutputPathBox.Text, $"(?<=){driveInfo.Name}\\").Success)
            .Contains(true) &&
            (from driveInfo
             in DriveInfo.GetDrives()
             select Regex.Match(PdfPathBox.Text, $"(?<=){driveInfo.Name}\\").Success)
            .Contains(true);

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            PdfList.Items.Clear();

            try
            {
                if (ConfirmButtonIsEnabled())
                {
                    var pdfFolderExists = Directory.Exists(PdfPathBox.Text);

                    if (pdfFolderExists)
                    {
                        var builder = new HammPdfWriter(OutputPathBox.Text);

                        var databaseExists = new FileInfo(OutputPathBox.Text).Exists;

                        if (!databaseExists)
                            builder.Build();

                        var pdfPaths = Directory.GetFiles(PdfPathBox.Text);

                        foreach (var pdfPath in pdfPaths)
                        {
                            var indicator = new WriteIndicator() { FileName = pdfPath };

                            PdfList.Items.Add(indicator);

                            var pdfReader = new PdfReader(pdfPath);
                            var reader = new HammPdfReader(pdfReader);

                            const int step = 10;

                            var numberOfPagesDiv = pdfReader.NumberOfPages / step;

                            int i = 0;

                            await Task.Run(() => {
                                for (; i < numberOfPagesDiv; i++)
                                    builder.Join(
                                        reader.GetExtendedDetails(i * step + 1, step),
                                        reader.GetModules(i * step + 1, step));

                                if (numberOfPagesDiv * step < pdfReader.NumberOfPages)
                                    builder.Join(
                                        reader.GetExtendedDetails(i * step + 1, pdfReader.NumberOfPages - numberOfPagesDiv * step),
                                        reader.GetModules(i * step + 1, pdfReader.NumberOfPages - numberOfPagesDiv * step));
                            });

                            indicator.IsDone = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("log.txt", ex.ToString());
            }
        }

        private void DatabasePathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderFileDialog = new FolderBrowserDialog();

            if (folderFileDialog.ShowDialog() == DialogResult.OK)
                OutputPathBox.Text = folderFileDialog.SelectedPath;
        }

        private void PdfPathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderFileDialog = new FolderBrowserDialog();

            if (folderFileDialog.ShowDialog() == DialogResult.OK)
                PdfPathBox.Text = folderFileDialog.SelectedPath;
        }

        private void PathBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConfirmButton.IsEnabled = ConfirmButtonIsEnabled();
        }
    }
}
