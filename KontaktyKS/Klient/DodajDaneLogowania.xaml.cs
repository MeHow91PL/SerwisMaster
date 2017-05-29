using SerwisMaster.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace SerwisMaster
{
    /// <summary>
    /// Interaction logic for dodanieDanychLogowania.xaml
    /// </summary>
    public partial class dodanieDanychLogowania : Window
    {
        List<DaneLogowaniaModel> list = null;
        public dodanieDanychLogowania(List<DaneLogowaniaModel> list, int system)
        {
            InitializeComponent();
            this.list = list;

            if (system > 0)
            {
                systemComboBox.SelectedIndex = system - 1;
                loginTextBox.Focus();
            }
            else systemComboBox.Focus();
        }

        private void zapiszButton_Click(object sender, RoutedEventArgs e)
        {
            zapiszDaneLogowania();
        }

        private void anulujButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void loginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            zapiszDaneLogowania();
        }          

        private void hasloTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapiszDaneLogowania();
        }

        private void zapiszDaneLogowania()
        {
            if (string.IsNullOrEmpty(systemComboBox.Text) ||
                string.IsNullOrWhiteSpace(loginTextBox.Text) ||
                string.IsNullOrWhiteSpace(hasloTextBox.Text))
            {
                MessageBox.Show("Uzupełnij wszystkie pola!");
            }
            else
            {
                //list.Add(new DaneLogowaniaModel() { system = systemComboBox.Text.Trim(), login = loginTextBox.Text.Trim(), haslo = hasloTextBox.Text });
                this.Close();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                wspolneOpcjeOkienek.zamknijOkienko(this);
        }
    }
}
