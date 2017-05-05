using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.IO;
using System.Windows.Data;
using System.Threading;
using System.Xml;
using System;
using KontaktyKS.Klasy_połączenia;

namespace KontaktyKS
{
    public abstract class Polaczenie : TreeViewItem
    {
        public string nazwa;
        public string haslo;
        public string typ;
        public string id;
        


        public Polaczenie(string id, string nazwa, string haslo, string typ)
        {
            this.id = id;
            this.nazwa = nazwa;
            this.Header = this.nazwa;
            this.haslo = haslo;
            this.typ = typ;
            this.ContextMenu = stworzContextMenu();
            this.MouseDoubleClick += Polaczenie_MouseDoubleClick;
            this.Selected += Polaczenie_Selected;
        }

        private void Polaczenie_Selected(object sender, RoutedEventArgs e)
        {
            Polaczenie polaczenia = sender as Polaczenie;

            
        }

        abstract public void Polaczenie_MouseDoubleClick(object sender, MouseButtonEventArgs e);

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

        void edytujPolaczenieClick(object sender, RoutedEventArgs e)
        {
            Polaczenie polaczenie = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as Polaczenie;
            OknoPolaczenia oknoPolaczenia = new OknoPolaczenia((polaczenie.Parent as Folder).id, id );
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

        void usunPolaczenie_Click( object sender, System.Windows.RoutedEventArgs e )
        {
            ContextMenu contextMenu = ( sender as MenuItem ).Parent as ContextMenu;
            Polaczenie polaczenie = contextMenu.PlacementTarget as Polaczenie;
            Klient klient = polaczenie.Parent as Klient;

            klient.Items.Remove(polaczenie);
            Thread nowyWatek = new Thread(usunPolaczenie);
            nowyWatek.Start( id );

            CollectionViewSource.GetDefaultView( klient.Items ).Refresh();
        }

        protected abstract void usunPolaczenie( object remoteId );
      
    }
}
