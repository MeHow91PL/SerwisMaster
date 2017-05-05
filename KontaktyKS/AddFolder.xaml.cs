using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KontaktyKS
{
    /// <summary>
    /// Interaction logic for AddFolder.xaml
    /// </summary>
    public partial class AddFolder : Window
    {
        Folder folder = null;

        public AddFolder(Folder folder)
        {
            InitializeComponent();
            this.folder = folder;
        }

        private void zapiszButton_Click(object sender, RoutedEventArgs e)
        {
            folder.nazwa = this.nazwaTextBox.Text;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dockPanel1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
