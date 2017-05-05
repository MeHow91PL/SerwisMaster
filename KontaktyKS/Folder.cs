using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Threading;
using System.Collections;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace KontaktyKS
{
    public class Folder : TreeViewItem
    {
        public string nazwa { get; set; }
        public object parent;
        public string id { get; set; }
        public string group { get; set; }

        public Folder() { }

        public Folder(string nazwa, object parent)
        {

            id = Guid.NewGuid().ToString();
            this.parent = parent;
            this.nazwa = nazwa;
            this.AllowDrop = true;
            Header = CreateHeader.createItemHeader(this);
            createContextMenu();

            this.MouseEnter += Folder_MouseEnter;
            this.MouseLeave += Folder_MouseLeave;
            this.GotFocus += Folder_GotFocus;

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

            mi[2].Header = "Zmień nazwę";
            mi[2].Click += new RoutedEventHandler(ZmienNazwe);

            mi[3].Header = "Usuń";
            mi[3].Click += new RoutedEventHandler(UsunFolder);

            this.ContextMenu = new ContextMenu();

            for (int i = 0; mi.Length > i; i++)
                this.ContextMenu.Items.Add(mi[i]);
        }

        protected void ZmienNazwe(object sender, RoutedEventArgs e)
        {
            focusOnFolder(this);
        }       

        public void DodajFolder(object sender, RoutedEventArgs e)
        {
            var parent = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget;
            Folder folder = new Folder("", parent);

            if (parent is Folder)
            {
                (parent as Folder).IsExpanded = true;
                parent.Focus();
                (parent as Folder).Items.Add(folder);
            }
            else
            {
                var list = (parent as TreeView).ItemsSource as List<Folder>;
                list.Add(folder);
                CollectionViewSource.GetDefaultView(list).Refresh();
            }



            Thread watek = new Thread(new ThreadStart(() => { Serializator.serializuj(folder); }));
            watek.Start();

            focusOnFolder(folder);

        }

        private void focusOnFolder(Folder f)
        {
            (f.Header as WrapPanel).Children.RemoveAt(1);
            (f.Header as WrapPanel).Children.Add(new MyTextBox(f) { Text = f.nazwa });

            TextBox tb = (f.Header as WrapPanel).Children[1] as TextBox;
            tb.Focusable = true;

            Dispatcher.BeginInvoke(DispatcherPriority.Input,
             new Action(delegate()
            {
                f.Focus();         // Set Logical Focus
                Keyboard.Focus(tb); // Set Keyboard Focus
            }));


        }

        protected virtual void UsunFolder(object sender, RoutedEventArgs e)
        {
            Folder folder = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as Folder;
            List<string> listaIdDoUsuniecia = null;

            if (MyMessageBox.Show("Czy na pewno chcesz usunąć bieżący element wraz z wszystkimi podległymi do niego?", "Usuń element",
                MyMessageBoxButtons.OkAnuluj) == MyResult.OK)
            {
                if (folder.Parent != null)
                {
                    ItemsControl folderParent = folder.Parent as ItemsControl;
                    listaIdDoUsuniecia = znajdzElementyPodlegle(folder);
                    folderParent.Items.Remove(folder);
                }
                else
                {
                    List<Folder> r = MainWindow.listOfClients.ItemsSource as List<Folder>;
                    listaIdDoUsuniecia = znajdzElementyPodlegle(folder);
                    r.Remove(folder);
                    CollectionViewSource.GetDefaultView(r).Refresh();
                }

                Thread newThread = new Thread(usun);
                newThread.Start(listaIdDoUsuniecia);
            }
        }

        private List<string> znajdzElementyPodlegle(Folder folder)
        {
            List<string> listaIdDoUsuniecia = new List<string>();

            Queue<Folder> kolejkaFolderow = new Queue<Folder>();
            kolejkaFolderow.Enqueue(folder);

            while (kolejkaFolderow.Count > 0) //wyszukuje wszystkie Folder do usunięcia
            {
                Folder tempFolder = kolejkaFolderow.Dequeue();
                listaIdDoUsuniecia.Add(tempFolder.id);
                if (tempFolder.HasItems)
                {
                    foreach (object o in tempFolder.Items)
                    {
                        if (o is Folder)
                            kolejkaFolderow.Enqueue(o as Folder);
                    }
                }
            }

            return listaIdDoUsuniecia;
        }

        private void usun(object obj)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Properties.Settings.Default.baseXmlPath);
            Queue<XmlNode> nodesToRemove = new Queue<XmlNode>();

            List<string> listaIdDoUsuniecia = obj as List<string>;
            

            XmlNodeList nodeList = xml["Connections"].ChildNodes;

            foreach(XmlNode node in nodeList)
            {
                if (listaIdDoUsuniecia.Contains(node.Attributes["Id"].InnerText))
                    nodesToRemove.Enqueue(node);
            }

            while(nodesToRemove.Count > 0)
            {
                XmlNode tempNode = nodesToRemove.Dequeue();
                tempNode.ParentNode.RemoveChild(tempNode);
            }

           
            xml.Save(Properties.Settings.Default.baseXmlPath);

        }
    }
}
