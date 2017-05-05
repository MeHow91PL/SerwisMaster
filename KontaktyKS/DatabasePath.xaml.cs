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
using System.Windows.Forms;

namespace KontaktyKS
{
    /// <summary>
    /// Interaction logic for Opcje.xaml
    /// </summary>
    public partial class Opcje : Window
    {
        public Opcje()
        {
            InitializeComponent();
            this.databasePathTextBox.Text = Properties.Settings.Default.DataBasePath;
        }

        private void button1_Click( object sender, RoutedEventArgs e )
        {
            this.Close();
        }

        private void dockPanel1_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            this.DragMove();
        }

        private void databasePathButton_Click( object sender, RoutedEventArgs e )
        {
            FolderBrowserDialog directory= new FolderBrowserDialog();
            directory.ShowDialog();
            this.databasePathTextBox.Text = directory.SelectedPath;
        }

        private void button2_Click( object sender, RoutedEventArgs e )
        {
            Properties.Settings.Default.DataBasePath = this.databasePathTextBox.Text;
            Properties.Settings.Default.Save();

            this.Close();
        }

        private void databasePathTextBox_MouseDoubleClick( object sender, MouseButtonEventArgs e )
        {
            this.databasePathTextBox.SelectAll();
        }
    }
}
