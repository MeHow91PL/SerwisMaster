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

namespace SerwisMaster
{
    /// <summary>
    /// Interaction logic for DodajTelefon.xaml
    /// </summary>
    public partial class DodajTelefon : Window
    {
        List<Telefon> tel;
        public DodajTelefon(List<Telefon> tel)
        {
            InitializeComponent();
            this.tel = tel;
            nazwaTextBox.Focus();
        }
        private void nazwaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapiszTelefon();
        }
        private void numerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapiszTelefon();
        }

        private void zapiszButton_Click(object sender, RoutedEventArgs e)
        {
            zapiszTelefon();
        }
        private void anulujButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void zapiszTelefon()
        {
            if (string.IsNullOrWhiteSpace(nazwaTextBox.Text) ||
                string.IsNullOrWhiteSpace(numerTextBox.Text))
                MessageBox.Show("Uzepełnij wszystkie pola!");
            else
            {
                tel.Add(new Telefon { nazwa = nazwaTextBox.Text, numer = numerTextBox.Text });
                this.Close();
            }
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

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                wspolneOpcjeOkienek.zamknijOkienko(this);
        }
    }
}
