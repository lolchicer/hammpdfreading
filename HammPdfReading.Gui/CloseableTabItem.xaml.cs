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

namespace InforHammPdfReading.Gui
{
    /// <summary>
    /// Логика взаимодействия для CloseableTabItemHeader.xaml
    /// </summary>
    public partial class CloseableTabItemHeader : UserControl
    {
        TabControl _parentTabControl;

        public CloseableTabItemHeader(TabControl parentTabControl, string name)
        {
            InitializeComponent();

            _parentTabControl = parentTabControl;
            NameBlock.Text = name;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _parentTabControl.Items.Remove(Parent);
        }
    }
}
