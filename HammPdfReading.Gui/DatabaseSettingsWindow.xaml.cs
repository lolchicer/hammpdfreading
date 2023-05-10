using System;
using System.Windows;
using System.Windows.Controls;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Csv;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Windows.Forms;
using System.Linq;
using System.Text.RegularExpressions;

namespace HammPdfReading.Gui
{
    /// <summary>
    /// Логика взаимодействия для DatabaseSettingsWindow.xaml
    /// </summary>
    public partial class DatabaseSettingsWindow : Window
    {
        string? _pdfPath;
        string _dbPath = string.Empty;
        int? _page;
        int? _count;

        public string DbPath { get { return _dbPath; } }

        public DatabaseSettingsWindow()
        {
            InitializeComponent();
        }

        bool BuildButtonIsEnabled() => 
            PathBox.Text != string.Empty &&
            (from driveInfo
             in System.IO.DriveInfo.GetDrives()
             select Regex.Match(PathBox.Text, $"(?<=){driveInfo.Name}\\").Success)
            .Contains(true);

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            _buildButton.IsEnabled = BuildButtonIsEnabled();
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            _dbPath = PathBox.Text + '\\' + NameBox.Text;

            new Builder(PathBox.Text + '\\' + NameBox.Text).Build();

            DialogResult = true;
        }

        private void PathOpen_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PathBox.Text = folderBrowserDialog.SelectedPath;
        }
    }
}
