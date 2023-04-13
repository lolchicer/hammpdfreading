using System;
using System.Windows;
using System.Windows.Controls;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Sqlite;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Test05
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

        public DatabaseSettingsWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox.Text == string.Empty)
                _buildButton.IsEnabled = false;
            else
                _buildButton.IsEnabled = true;
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            new Builder(_dbPath).Build();
        }
    }
}
