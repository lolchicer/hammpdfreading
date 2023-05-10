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

namespace HammPdfReading.Gui
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        MainWindow _window;

        public MainPage(MainWindow window)
        {
            InitializeComponent();

            _window = window;
        }

        private void NewDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            _window.NewDatabase();
        }

        private void OpenDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            _window.OpenDatabase();
        }
    }
}
