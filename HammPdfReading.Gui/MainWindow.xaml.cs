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

namespace Infor.HammPdfReading.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        void OpenHome()
        {
            DatabaseTabControl.Items.Add(new TabItem { Content = new Frame() { Content = new MainPage(this) }, Header = new CloseableTabItemHeader(DatabaseTabControl, "Домашняя вкладка") });
        }

        public MainWindow()
        {
            InitializeComponent();

            OpenHome();
        }

        public void NewDatabase()
        {
            DatabaseSettingsWindow window = new DatabaseSettingsWindow();

            if (window.ShowDialog() == true)
            {
                DatabaseTabControl.Items.Add(new TabItem { Content = new Frame() { Content = new DatabaseBuilderPage(window.DbPath) }, Header = new CloseableTabItemHeader(DatabaseTabControl, window.DbPath) });
            }
        }

        public void OpenDatabase()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName;

                DatabaseTabControl.Items.Add(new TabItem { Content = new Frame() { Content = new DatabaseBuilderPage(path) }, Header = new CloseableTabItemHeader(DatabaseTabControl, path) });
            }
        }

        private void FileNew_Click(object sender, RoutedEventArgs e) => NewDatabase();

        private void FileOpen_Click(object sender, RoutedEventArgs e) => OpenDatabase();

        private void ViewHome_Click(object sender, RoutedEventArgs e) => OpenHome();
    }
}
