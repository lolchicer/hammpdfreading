﻿using System;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName;

                new DatabaseWindow(path).Show();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DatabaseSettingsWindow window = new DatabaseSettingsWindow();

            if (window.ShowDialog() == true)
            {
                new DatabaseWindow(window.DbPath).Show();
            }
        }
    }
}