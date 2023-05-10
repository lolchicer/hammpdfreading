using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Win32;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Csv;
using iTextSharp.text.pdf;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using iTextSharp.text.pdf.parser;

namespace HammPdfReading.Gui
{
    /// <summary>
    /// Логика взаимодействия для DatabaseBuilderPage.xaml
    /// </summary>
    public partial class DatabaseBuilderPage : Page
    {
        Builder _builder;

        public DatabaseBuilderPage(string path)
        {
            _builder = new Builder(path);

            InitializeComponent();
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
                PdfTabControl.Items.Add(new TabItem() { Content = new Frame() { Content = new PdfPage(openFileDialog.FileName, _builder) }, Header = new CloseableTabItemHeader(PdfTabControl, openFileDialog.FileName) });
        }
    }
}
