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
using System.Xml;

namespace SerwisMaster
{
    /// <summary>
    /// Interaction logic for DatabasePath.xaml
    /// </summary>
    public partial class DatabasePath : Window
    {
        public object XmlDocumnet { get; private set; }

        public DatabasePath()
        {
            InitializeComponent();
            this.databasePathTextBox.Text = Properties.Settings.Default.baseXmlPath;
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
            OpenFileDialog file= new OpenFileDialog();
            file.Filter = "Pliki xml|*.xml";
            file.ShowDialog();
            this.databasePathTextBox.Text = file.FileName;
        }

        private void button2_Click( object sender, RoutedEventArgs e )
        {
            string dbPath = databasePathTextBox.Text;

            if (!MainWindow.sprawdzKluczBazyDanych(dbPath))
                return;


            Properties.Settings.Default.baseXmlPath = dbPath;
           Properties.Settings.Default.Save();
            MainWindow.sprawdzenieBazyDanych();
            MainWindow.aktualizujTreeView(MainWindow.listOfClients);
            this.Close();
        }

        private void databasePathTextBox_MouseDoubleClick( object sender, MouseButtonEventArgs e )
        {
            this.databasePathTextBox.SelectAll();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                wspolneOpcjeOkienek.zamknijOkienko(this);
        }
    }
}
