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
using Infor.HammPdfReading.Csv;
using iTextSharp.text.pdf;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HammPdfReading.Gui
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

        int _index = 1;
        int _count = 1;

        private void PageIndexBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int index;

            bool isRight = true;

            if (int.TryParse(((TextBox)sender).Text, out index))
                if (index > 0)
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
                if (count > 0)
                    _count = count;
                else
                    isRight = false;
            else
                isRight = false;

            if (!isRight)
                ((TextBox)sender).Text = _count.ToString();
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
                PathBox.Text = openFileDialog.FileName;
        }

        bool InsertButtonIsEnabled() => 
            (from driveInfo
             in System.IO.DriveInfo.GetDrives()
             select Regex.Match(PathBox.Text, $"(?<=){driveInfo.Name}\\").Success)
            .Contains(true);

        private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InsertButton.IsEnabled = InsertButtonIsEnabled();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var reader = new Reader(new PdfReader(PathBox.Text));

            var page = Convert.ToInt32(PageIndexBox.Text);
            var count = Convert.ToInt32(PageCountBox.Text);

            _builder.Join(reader.ExtendedDetails(page, count).ToArray(), reader.GetModules(page, count));
        }
    }
}
