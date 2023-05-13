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

namespace Infor.HammPdfReading.Gui
{
    /// <summary>
    /// Логика взаимодействия для WriteIndicator.xaml
    /// </summary>
    public partial class WriteIndicator : UserControl
    {
        public string FileName { get { return FileNameBlock.Text; } set { FileNameBlock.Text = value; } }
        bool _isDone = false;
        public bool IsDone 
        { 
            get { return _isDone; } 
            set 
            {
                _isDone = value;
                if (value)
                    IsDoneBox.Text = "Выполнено";
                else
                    IsDoneBox.Text = "Не выполнено";
            }
        }

        public WriteIndicator()
        {
            InitializeComponent();
        }
    }
}
