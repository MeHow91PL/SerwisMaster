using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SerwisMaster
{
    /// <summary>
    /// Interaction logic for OknoWebBrowser.xaml
    /// </summary>
    public partial class OknoWebBrowser : Window
    {
        public OknoWebBrowser()
        {
            InitializeComponent();
        }

        private void addressTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapiszWebBrowser();
        }

        private void numerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapiszWebBrowser();
        }

        private void zapiszButton_Click(object sender, RoutedEventArgs e)
        {
            zapiszWebBrowser();
        }
        private void anulujButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dockPanel1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void minimalizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void zapiszWebBrowser()
        {
            throw new NotImplementedException();
        }

        private void webBrowserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void defaultWebBrowserCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            webBrowserComboBox.IsEnabled = false;


            Process.Start(addressTextBox.Text);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                wspolneOpcjeOkienek.zamknijOkienko(this);
        }
    }
}
