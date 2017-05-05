using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.IO;
using System.Windows.Data;
using System.Threading;
using System.Xml;
using System;
using SerwisMaster.Klasy_połączenia;

namespace SerwisMaster
{
    public abstract class Polaczenie : Element
    {
        public string haslo;
        public string typ;

        public Polaczenie(string nazwa, string group, string opis, string haslo, string typ,string id = "", object parent = null) : base(nazwa,group,opis,id,parent)
        {
            this.Tag = "3" + nazwa;
            this.Header = this.nazwa;
            this.haslo = haslo;
            this.typ = typ;
            this.ContextMenu = stworzContextMenu();
            this.MouseDoubleClick += uruchomPolaczenie;
            this.Selected += Polaczenie_Selected;
            this.KeyDown +=  Polaczenie_KeyDown;
        }

        private void Polaczenie_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                uruchomPolaczenie(sender, null);
            }
        }

        private void Polaczenie_Selected(object sender, RoutedEventArgs e)
        {
            Polaczenie polaczenia = sender as Polaczenie;

            
        }

        abstract public void uruchomPolaczenie(object sender, MouseButtonEventArgs e);

        private ContextMenu stworzContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem[] items = new MenuItem[2];

            for( int i = 0; i < items.Length; i++ )
                items[i] = new MenuItem();

            items[0].Header = "Edytuj";
            items[0].Click += edytujPolaczenieClick;

            items[1].Header = "Usuń";
            items[1].Click += usunPolaczenie_Click;

            foreach(MenuItem menuItem in items)
            cm.Items.Add(menuItem);

            return cm;
        }

        public void edytujPolaczenieClick(object sender, RoutedEventArgs e)
        {
            Polaczenie polaczenie = getSenderParent(sender) as Polaczenie;
            OknoPolaczenia oknoPolaczenia = new OknoPolaczenia((polaczenie.Parent as Folder).id, polaczenie.id );
            oknoPolaczenia.nazwaTextBox.Text = polaczenie.nazwa;
            oknoPolaczenia.hasloTextBox.Text = polaczenie.haslo;
            oknoPolaczenia.rodzajComboBox.IsEnabled = false;

            if (polaczenie is TeamViewer)
            {
                oknoPolaczenia.rodzajComboBox.SelectedIndex = 0;
                oknoPolaczenia.loginTextBox.Text = (polaczenie as TeamViewer).teamViewerId;
            }
            else if (polaczenie is Rdp)
            {
                oknoPolaczenia.rodzajComboBox.SelectedIndex = 1;
                oknoPolaczenia.loginTextBox.Text = (polaczenie as Rdp).login;
                oknoPolaczenia.adresTextBox.Text = (polaczenie as Rdp).adresRDP;
            }

            oknoPolaczenia.Show();
        }

        public void usunPolaczenie_Click( object sender, System.Windows.RoutedEventArgs e )
        {
            Polaczenie polaczenie = getSenderParent(sender) as Polaczenie;
            Klient klient = polaczenie.Parent as Klient;

            if (MyMessageBox.Show("Czy na pewno chcesz usunąć to połączenie?", "Usuń element",
                MyMessageBoxButtons.OkAnuluj) == MyResult.OK)
            {

                klient.Items.Remove(polaczenie);
                Thread nowyWatek = new Thread(usunPolaczenie);
                nowyWatek.Start(polaczenie.id);

                CollectionViewSource.GetDefaultView(klient.Items).Refresh();
            }
        }

        protected abstract void usunPolaczenie( object remoteId );
      
    }
}
