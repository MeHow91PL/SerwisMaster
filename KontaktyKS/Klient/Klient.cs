using SerwisMaster.BL;
using SerwisMaster.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SerwisMaster
{
    public class Klient : Folder, IDisposable
    {

        static IBazaDanych db = new BazaLocalDb();

        public List<EmailModel> emailList;
        public List<TelefonModel> telefonList;
        public List<DaneLogowaniaModel> daneLogowaniaList;

        public Klient(string nazwa, string kluczRodzica, string opis, List<EmailModel> emailList, 
            List<TelefonModel> telefonList, List<DaneLogowaniaModel> daneLogowaniaList, string klucz="", object parent=null)
            : base(nazwa, kluczRodzica, opis,klucz, parent)
        {
            this.Rodzaj = RodzajElementu.Klient;
            this.Tag = "1" + nazwa;
            this.emailList = emailList;
            this.telefonList = telefonList;
            this.daneLogowaniaList = daneLogowaniaList;
            //utworzContextMenu();
        }
       
        public static void DodajKlienta(object sender, RoutedEventArgs e)
        {
            var parent = getSenderParent(sender);
            string kluczRodzica = "";

            if (parent is Element)
            {
                kluczRodzica = (parent as Element).Klucz;
            }

            Klient klient = new Klient("", kluczRodzica, "", new List<EmailModel>(), new List<TelefonModel>(), new List<DaneLogowaniaModel>());

            DaneKlienta newClient = new DaneKlienta(klient, sender);
            newClient.ShowDialog();
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

            for (int i = 0; mi.Length > i; i++)
                contextMenu.Items.Add(mi[i]);

            this.ContextMenu = contextMenu;
        }


        public void Klient__DodajPolaczenie_Click(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = true;

            Klient klient = getSenderParent(sender) as Klient;
            OknoPolaczenia dodajPolaczenie = new OknoPolaczenia(klient.Klucz);
            dodajPolaczenie.ShowDialog();

            MainWindow.aktualizujTreeView(MainWindow.listOfClients);
        }

        public void Klient_Edytuj_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Klient klient = getSenderParent(sender) as Klient;

            DaneKlienta edit = new DaneKlienta(klient);
            edit.ShowDialog();
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
                Nazwa = string.Empty;
                emailList = null;
                telefonList = null;
                daneLogowaniaList = null;
            }
        }

        public static implicit operator Klient(KlientModel klientModel)
        {
            Klient klient = new Klient(
                klientModel.Element.Nazwa,
                klientModel.Element.KluczRodzica,
                klientModel.Element.Opis,
                klientModel.AdresyEmail,
                klientModel.Telefony,
                klientModel.DaneLogowania,
                klientModel.Element.Klucz
            );
            return klient;
        }

    }
}
