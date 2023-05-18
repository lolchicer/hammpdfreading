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

namespace Infor.HammPdfReading.Gui
{
    /// <summary>
    /// Логика взаимодействия для FormPage.xaml
    /// </summary>
    public partial class FormPage : Page
    {
        public FormPage()
        {
            InitializeComponent();
        }

        bool ConfirmButtonIsEnabled() =>
            (from driveInfo
             in DriveInfo.GetDrives()
             select Regex.Match(DatabasePathBox.Text, $"(?<=){driveInfo.Name}\\").Success)
            .Contains(true) &&
            (from driveInfo
             in DriveInfo.GetDrives()
             select Regex.Match(PdfPathBox.Text, $"(?<=){driveInfo.Name}\\").Success)
            .Contains(true);

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            PdfList.Items.Clear();

            if (ConfirmButtonIsEnabled())
            {
                var pdfFolderExists = Directory.Exists(PdfPathBox.Text);

                if (pdfFolderExists)
                {
                    var builder = new HammPdfWriter(DatabasePathBox.Text);

                    var databaseExists = new FileInfo(DatabasePathBox.Text).Exists;

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

        private void DatabasePathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "CSV (*.csv)|*.csv|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                DatabasePathBox.Text = openFileDialog.FileName;
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
