using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KontaktyKS.Klasy_połączenia;
using System.IO;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Xml;

namespace KontaktyKS
{
    public class Klient : Folder, IDisposable
    {
        public List<Email> emailList;
        public List<Telefon> telefonList;
        public List<DaneLogowania> daneLogowaniaList;

        public Klient(string nazwa, List<Email> emailList, List<Telefon> telefonList,
            List<DaneLogowania> daneLogowaniaList, object parent)
            : base(nazwa, parent)
        {
            this.emailList = emailList;
            this.telefonList = telefonList;
            this.daneLogowaniaList = daneLogowaniaList;
            utworzContextMenu();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                nazwa = string.Empty;
                emailList = null;
                telefonList = null;
                daneLogowaniaList = null;
            }
        }



        public static void DodajKlienta(object sender, RoutedEventArgs e)
        {
            var parent = getSenderParent(sender);

           
            
            DodajKlienta newClient = new DodajKlienta(parent);
            newClient.ShowDialog();
        }

        private static object getSenderParent(object sender)
        {
            if (sender is MenuItem)
            {
                var mi = (MenuItem)sender;
                var cm = (ContextMenu)mi.Parent;
                return cm.PlacementTarget;
            }
            else
            {
                return sender;
            }
        }

        private void utworzContextMenu()
        {

            ContextMenu contextMenu = new ContextMenu();
            MenuItem[] mi = new MenuItem[5];

            for (int i = 0; mi.Length > i; i++)
                mi[i] = new MenuItem();

            mi[0].Header = "Dodaj połączenie";
            mi[0].Click += Klient__DodajPolaczenie_Click;

            mi[1].Header = "Dodaj folder";
            mi[1].Click += DodajFolder;

            mi[2].Header = "Edytuj";
            mi[2].Click += Klient_Edytuj_Click;

            mi[3].Header = "Zmień nazwę";
            mi[3].Click += base.ZmienNazwe; 
            
            mi[4].Header = "Usuń";
            mi[4].Click += UsunFolder;

            for (int i = 0; mi.Length > i; i++)
                contextMenu.Items.Add(mi[i]);

            this.ContextMenu = contextMenu;
        }


        void Klient__DodajPolaczenie_Click(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = true;

            var mi = (MenuItem)sender;
            var cm = (ContextMenu)mi.Parent;
            var klient = (Klient)cm.PlacementTarget;
            OknoPolaczenia dodajPolaczenie = new OknoPolaczenia(klient.id);
            dodajPolaczenie.ShowDialog();

            MainWindow.aktualizujTreeView(MainWindow.listOfClients);
        }

        void Klient_Edytuj_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            var cm = (ContextMenu)mi.Parent;
            var klient = (Klient)cm.PlacementTarget;
            EdytujKlienta edit = new EdytujKlienta(klient);
            edit.ShowDialog();
        }


    }



    public class Email : MenuItem
    {
        public string adresEmail { get; set; }
    }


    public class Telefon
    {
        public string nazwa { get; set; }
        public string numer { get; set; }
    }

    public class DaneLogowania
    {
        public string login { get; set; }
        public string haslo { get; set; }
        public string system { get; set; }
    }
}
