using Infor.HammPdfReading;
using Infor.HammPdfReading.Csv;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace HammPdfReading.Gui
{
    /// <summary>
    /// Логика взаимодействия для PdfPage.xaml
    /// </summary>
    public partial class PdfPage : Page
    {
        PdfReader _pdfReader;
        HammPdfReader _reader;

        Builder _builder;

        int _numberOfPages;

        int _index = 1;
        int _count = 1;

        public PdfPage(string path, Builder builder)
        {
            _pdfReader = new PdfReader(path);
            _reader = new HammPdfReader(_pdfReader);

            _numberOfPages = _pdfReader.NumberOfPages;

            _builder = builder;

            InitializeComponent();
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            var page = Convert.ToInt32(PageIndexBox.Text);
            var count = Convert.ToInt32(PageCountBox.Text);

            _builder.Join(_reader.ExtendedDetails(page, count).ToArray(), _reader.GetModules(page, count));
        }

        private void PageIndexBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int index;

            bool isRight = true;

            if (int.TryParse(((TextBox)sender).Text, out index))
                if (index > 0 && index <= _numberOfPages)
                    _index = index;
                else
                    isRight = false;
            else
                isRight = false;

            if (!isRight)
                ((TextBox)sender).Text = _index.ToString();
        }

        private void PageCountBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int count;

            bool isRight = true;

            if (int.TryParse(((TextBox)sender).Text, out count))
                if (count > 0 && count <= _numberOfPages + 1 - _index)
                    _count = count;
                else
                    isRight = false;
            else
                isRight = false;

            if (!isRight)
                ((TextBox)sender).Text = _count.ToString();
        }
    }
}
