using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls.Primitives;


namespace SerwisMaster
{
    /// <summary>
    /// Logika interakcji dla klasy Dodaj_klienta.xaml
    /// </summary>
    public abstract partial class DaneKlienta : Window
    {
        public CollectionView daneLogowaniaView, emailView;
        public List<DaneLogowania> daneLogowaniaList;
        public List<Email> emailList;
        public List<Telefon> telefonList;
        protected object parent = null;

        public DaneKlienta(object parent)
        {
          
            this.parent = parent;
            InitializeComponent();
            this.emailList = new List<Email>();
            this.telefonList = new List<Telefon>();
            this.daneLogowaniaList = new List<DaneLogowania>();
            ustawZrodlaList();
        }

        protected void ustawZrodlaList()
        {
            try
            {
                daneLogowaniaListView.ItemsSource = this.daneLogowaniaList;
                daneLogowaniaView = (CollectionView)CollectionViewSource.GetDefaultView(daneLogowaniaListView.ItemsSource);
                daneLogowaniaView.Filter = daneLogowaniaFilter;

                emailListView.ItemsSource = this.emailList;
                emailView = (CollectionView)CollectionViewSource.GetDefaultView(emailListView.ItemsSource);
                emailView.Filter = emailFilter;

                telefonyListView.ItemsSource = this.telefonList;
                this.nazwaTextBox.Focus();
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }



        private void nazwaTB_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapiszKlienta();
        }


        //################################ Email #########################################################

        private void emailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (string.IsNullOrWhiteSpace((sender as TextBox).Text)) zapiszKlienta();
                    else dodajEmail();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }
        private void dodajemailButton_Click(object sender, RoutedEventArgs e)
        {
            dodajEmail();
        }

        private void dodajEmail()
        {
            try
            {
                string pattern = @"^[a-ząćęłńóśźżA-ZĄĆĘŁŃÓŚŹŻ0-9][a-ząćęłńóśźżA-ZĄĆĘŁŃÓŚŹŻ0-9._]*@[a-z0-9]*\.[a-z]{2,4}$";
                Regex reg = new Regex(pattern);

                if (reg.IsMatch(emailTextBox.Text))
                {
                    emailList.Add(new Email() { adresEmail = emailTextBox.Text });
                    CollectionViewSource.GetDefaultView(emailList).Refresh();
                    emailTextBox.Clear();
                }
                else
                    System.Windows.MessageBox.Show("Niepoprawny email!");
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void usunEmailButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (emailListView.SelectedItem != null)
                {
                    emailList.Remove((emailListView.SelectedItem as Email));
                    CollectionViewSource.GetDefaultView(emailListView.ItemsSource).Refresh();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void emailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (emailListView.Items.Count != 0)
            {
                CollectionViewSource.GetDefaultView(emailListView.ItemsSource).Refresh();
            }
        }

        private bool emailFilter(object item) //filtr który zwraca każy wiersz emailListView gdy pole email jest puste lub zawiera pasujący ciąg znaków
        {
            if (string.IsNullOrEmpty(emailTextBox.Text))
            {
                return true;
            }
            else
                return (item as Email).adresEmail.Contains(emailTextBox.Text);
        }




        //################################ Telefon #########################################################

        private void dodajTelefonButton_Click(object sender, RoutedEventArgs e)
        {
            DodajTelefon tel = new DodajTelefon(this.telefonList);
            tel.ShowDialog();
            CollectionViewSource.GetDefaultView(telefonyListView.ItemsSource).Refresh();
        }

        private void usunTelefonButton_Click(object sender, RoutedEventArgs e)
        {
            if (telefonyListView.SelectedItem != null)
                telefonList.Remove((Telefon)telefonyListView.SelectedItem);
            CollectionViewSource.GetDefaultView(telefonyListView.ItemsSource).Refresh();
        }


        //################################ Dane logowania #########################################################

        private void systemComboBox_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
        }

        private void dodajDaneLogowania_Click( object sender, RoutedEventArgs e )
        {
            dodajDaneLogowania();
        }

        private void usunDaneLogoeanie_Click(object sender, RoutedEventArgs e)
        {
            daneLogowaniaList.Remove((daneLogowaniaListView.SelectedItem as DaneLogowania));
            CollectionViewSource.GetDefaultView(daneLogowaniaListView.ItemsSource).Refresh();
        }

        private bool daneLogowaniaFilter(object item)
        {
            if (String.IsNullOrWhiteSpace(systemComboBox.Text) || String.Equals(systemComboBox.Text, "<wszystko>"))
                return true;
            else
                return (string.Equals((item as DaneLogowania).system.ToUpper(), systemComboBox.Text.ToUpper()));
        }

   
        private void ComboBoxItem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (daneLogowaniaView != null)
            {
                CollectionViewSource.GetDefaultView( daneLogowaniaListView.ItemsSource ).Refresh();
            }
        }

        private void dodajKlientaButton_Click( object sender, RoutedEventArgs e )
        {
            zapiszKlienta();
        }

        private void anulujButton_Click( object sender, RoutedEventArgs e )
        {
            this.Close();
        }

        protected abstract void zapiszKlienta();

        private void systemComboBox_KeyDown( object sender, KeyEventArgs e )
        {
            if( e.Key == Key.Enter )
                dodajDaneLogowania();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                wspolneOpcjeOkienek.zamknijOkienko(this);
        }

        private void dodajDaneLogowania()
        {
            dodanieDanychLogowania log = new dodanieDanychLogowania( daneLogowaniaList, systemComboBox.SelectedIndex );
            log.ShowDialog();
            CollectionViewSource.GetDefaultView( daneLogowaniaListView.ItemsSource ).Refresh();
        }


    }
}
