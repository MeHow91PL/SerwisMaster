using SerwisMaster.BL;
using SerwisMaster.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;


namespace SerwisMaster
{
    /// <summary>
    /// Logika interakcji dla klasy Dodaj_klienta.xaml
    /// </summary>
    public partial class DaneKlienta : Window
    {
        static IBazaDanych db = new BazaLocalDb();


        public CollectionView daneLogowaniaView, emailView;
        public List<DaneLogowaniaModel> daneLogowaniaList;
        public List<EmailModel> emailList;
        public List<TelefonModel> telefonList;
        public Klient klient;
        private object parent;

        public DaneKlienta(Klient klient, object parent = null)
        {
            InitializeComponent();
            this.klient = klient;
            this.emailList = new List<EmailModel>();
            this.telefonList = new List<TelefonModel>();
            this.daneLogowaniaList = new List<DaneLogowaniaModel>();
            this.parent = parent;
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

        private void zapiszKlienta()
        {
            this.klient.Nazwa = nazwaTextBox.Text;
            this.klient.emailList = this.emailList;
            this.klient.telefonList = this.telefonList;
            this.klient.daneLogowaniaList = this.daneLogowaniaList;
            this.klient.Opis = new TextRange(opisRichTextbox.Document.ContentStart, opisRichTextbox.Document.ContentEnd).Text;

            if (db.CzyElementJuzIstnieje(AtrybutElementu.Klucz, klient)) //jeżeli klient istnieje to edytuj
            {
                db.EdytujElement(klient);
                this.Close();
            }
            //else if(db.CzyElementJuzIstnieje(AtrybutElementu.Nazwa, klient)) //Sprawdzenie czy nie istnieją już elementy o podanej nazwie i pytanie usera o decyzję
            //{
            //    if(MyMessageBox.Show("Na liście istnieją już elementy o podanej nazwie. Na pewno chcesz zapisać tego klienta?",
            //    "Klient już istnieje", MyMessageBoxButtons.TakNie) == MessageBoxResult.TAK)
            //    {

            //        db.DodajElement(klient);
            //        this.Close();
            //    }
            //}
            else
            {
                if (parent is Element)
                {
                    (parent as Element).IsExpanded = true;
                    (parent as Element).Items.Add(klient);
                }
                else
                {
                    var list = (parent as TreeView).ItemsSource as List<Element>;
                    list.Add(klient);
                    CollectionViewSource.GetDefaultView(list).Refresh();
                }
                   
                Thread watekDodawaniaElementu = new Thread(dodajElementAsync);
                watekDodawaniaElementu.Start(klient);
                this.Close();
            }
        }

        private void dodajElementAsync(object obj)
        {
            Klient klient = obj as Klient;
            db.DodajElement(klient);
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
                    emailList.Add(new EmailModel() { AdresEmail = emailTextBox.Text });
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
                    emailList.Remove((emailListView.SelectedItem as EmailModel));
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
                return (item as EmailModel).AdresEmail.Contains(emailTextBox.Text);
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
                telefonList.Remove((TelefonModel)telefonyListView.SelectedItem);
            CollectionViewSource.GetDefaultView(telefonyListView.ItemsSource).Refresh();
        }

        //################################ Dane logowania #########################################################

        private void systemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void dodajDaneLogowania_Click(object sender, RoutedEventArgs e)
        {
            dodajDaneLogowania();
        }

        private void usunDaneLogoeanie_Click(object sender, RoutedEventArgs e)
        {
            daneLogowaniaList.Remove((daneLogowaniaListView.SelectedItem as DaneLogowaniaModel));
            CollectionViewSource.GetDefaultView(daneLogowaniaListView.ItemsSource).Refresh();
        }

        private bool daneLogowaniaFilter(object item)
        {
            if (String.IsNullOrWhiteSpace(systemComboBox.Text) || String.Equals(systemComboBox.Text, "<wszystko>"))
                return true;
            else
                //return (string.Equals((item as DaneLogowaniaModel).System.ToUpper(), systemComboBox.Text.ToUpper()));
                return false;
        }

        private void ComboBoxItem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (daneLogowaniaView != null)
            {
                CollectionViewSource.GetDefaultView(daneLogowaniaListView.ItemsSource).Refresh();
            }
        }

        private void dodajKlientaButton_Click(object sender, RoutedEventArgs e)
        {
            zapiszKlienta();
        }

        private void anulujButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void systemComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                dodajDaneLogowania();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                wspolneOpcjeOkienek.zamknijOkienko(this);
        }

        private void dodajDaneLogowania()
        {
            dodanieDanychLogowania log = new dodanieDanychLogowania(daneLogowaniaList, systemComboBox.SelectedIndex);
            log.ShowDialog();
            CollectionViewSource.GetDefaultView(daneLogowaniaListView.ItemsSource).Refresh();
        }
    }
}
