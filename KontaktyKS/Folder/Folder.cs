using SerwisMaster.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace SerwisMaster
{
    public class Folder : Element
    {
        public string groupRDM { get; set; }

        public Folder() : base() { }

        public Folder(string nazwa, string kluczRodzica, string opis, string klucz = "", object parent = null)
            : base(nazwa, kluczRodzica, opis, klucz, parent)
        {
            this.Rodzaj = RodzajElementu.Folder;
            this.Tag = "1" + nazwa;
            this.AllowDrop = true;
            //createContextMenu();
            this.MouseEnter += Folder_MouseEnter;
            this.MouseLeave += Folder_MouseLeave;
            this.GotFocus += Folder_GotFocus;
            this.Expanded += Tescik;
        }

        private void Tescik(object sender, RoutedEventArgs e)
        {
            this.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Tag", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void Folder_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Folder_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        void Folder_GotFocus(object sender, RoutedEventArgs e)
        {
            WrapPanel wp = this.Header as WrapPanel;
            wp.Children[1].Focus();
        }

        private void createContextMenu()
        {
            MenuItem[] mi = new MenuItem[4];

            for (int i = 0; i < mi.Length; i++)
            {
                mi[i] = new MenuItem();
            }

            mi[0].Header = "Dodaj folder";
            mi[0].Click += new RoutedEventHandler(DodajFolder);

            mi[1].Header = "Dodaj klienta";
            mi[1].Click += new RoutedEventHandler(Klient.DodajKlienta);

            this.ContextMenu = new ContextMenu();

            for (int i = 0; mi.Length > i; i++)
                this.ContextMenu.Items.Add(mi[i]);
        }


        public void DodajFolder(object sender, RoutedEventArgs e)
        {
            var parent = getSenderParent(sender) as UIElement;
            Folder folder = new Folder("", "", "", "", parent);

            if (parent is Folder)
            {
                (parent as Folder).IsExpanded = true;
                parent.Focus();
                (parent as Folder).Items.Add(folder);
            }
            else
            {
                var list = (parent as TreeView).ItemsSource as List<Element>;
                list.Add(folder);
                CollectionViewSource.GetDefaultView(list).Refresh();
            }
            //Thread watek = new Thread(new ThreadStart(() => { Serializator.serializuj(folder); }));
            //watek.Start();
            focusOnFolder(folder);
        }

        public void ZmienNazwe(object sender, RoutedEventArgs e)
        {
            focusOnFolder(this);
        }

        private void focusOnFolder(Folder f)
        {
            (f.Header as WrapPanel).Children.RemoveAt(1);
            (f.Header as WrapPanel).Children.Add(new MyTextBox(f) { Text = f.Nazwa, VerticalAlignment = VerticalAlignment.Center });

            TextBox tb = (f.Header as WrapPanel).Children[1] as TextBox;
            tb.Focusable = true;

            Dispatcher.BeginInvoke(DispatcherPriority.Input,
             new Action(delegate ()
            {
                f.Focus();         // Set Logical Focus
                Keyboard.Focus(tb); // Set Keyboard Focus
            }));
        }
    }
}
