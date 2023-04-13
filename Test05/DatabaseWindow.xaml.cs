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
using System.Windows.Shapes;
using Microsoft.Win32;
using Infor.HammPdfReading;
using Infor.HammPdfReading.Sqlite;
using iTextSharp.text.pdf;

namespace Test05
{
    /// <summary>
    /// Логика взаимодействия для DatabaseWindow.xaml
    /// </summary>
    public partial class DatabaseWindow : Window
    {
        Builder _builder;

        public DatabaseWindow(string path)
        {
            _builder = new Builder(path);

            InitializeComponent();
        }

        private void PageIndexBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void PageCountBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
                PathBox.Text = openFileDialog.FileName;
        }

        private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Numeric(object sender, TextCompositionEventArgs e)
        {
            int апрвпавеквпситма;

            if (!int.TryParse(e.Text, out апрвпавеквпситма))
                e.Handled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var reader = new Reader(new PdfReader(PathBox.Text));

            _builder.Insert(reader.ExtendedDetails(Convert.ToInt32(PageIndexBox.Text), Convert.ToInt32(PageCountBox.Text)).ToArray());
        }
    }
}
