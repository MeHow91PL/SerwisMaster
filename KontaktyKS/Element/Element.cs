using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace SerwisMaster
{
    [Table("Elementy")]
    public abstract class Element : TreeViewItem
    {
        public string nazwa { get; set; }
        public string id { get; set; }
        public string group { get; set; }
        public string opis { get; set; }
        public object parent;

        public Element()
        {
            this.nazwa = "";
            this.id = "";
            this.group = "";
            this.opis = "";
            this.parent = null;
        }
        
        public Element(string nazwa, string group, string opis, string id = "", object parent = null)
        {
            this.nazwa = nazwa;

            if (string.IsNullOrWhiteSpace(id))
                id = Guid.NewGuid().ToString();
            else
                this.id = id;

            this.group = group;
            this.opis = opis;
            this.parent = parent;

            this.Header = CreateHeader.createItemHeader(this);
            this.ContextMenu = CreateContextMenu();
        }

        protected virtual ContextMenu CreateContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Zmień nazwę";
            menuItem.Click += ZmienNazwe_Click;
            contextMenu.Items.Add(menuItem);

            MenuItem menuItem2 = new MenuItem();
            menuItem2.Header = "Usuń";
            menuItem2.Click += UsunElement_Click;
            contextMenu.Items.Add(menuItem2);

            return contextMenu;
        }

        private void ZmienNazwe_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UsunElement_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UsunElement(sender);
        }

        public void UsunElement(object sender)
        {
            Element element = getSenderParent(sender) as Element;
            List<string> listaIdDoUsuniecia = null;

            if (MyMessageBox.Show("Czy na pewno chcesz usunąć bieżący element wraz z wszystkimi podległymi do niego?", "Usuń element",
                MyMessageBoxButtons.OkAnuluj) == MyResult.OK)
            {
                if (element.Parent != null) //Jeżeli elementem nadżędnym do usuwanego jest inny element
                {
                    ItemsControl folderParent = element.Parent as ItemsControl;
                    listaIdDoUsuniecia = znajdzElementyPodlegle(element);
                    folderParent.Items.Remove(element);
                }
                else //Jeżeli tym elementem jest ListView
                {
                    List<Element> r = MainWindow.listOfClients.ItemsSource as List<Element>;
                    listaIdDoUsuniecia = znajdzElementyPodlegle(element);
                    r.Remove(element);
                    CollectionViewSource.GetDefaultView(r).Refresh();
                }

                Thread newThread = new Thread(usun);
                newThread.Start(listaIdDoUsuniecia);
            }
        }

        private List<string> znajdzElementyPodlegle(Element element)
        {
            List<string> listaIdDoUsuniecia = new List<string>();

            Queue<Element> kolejkaElementow = new Queue<Element>();
            kolejkaElementow.Enqueue(element);

            while (kolejkaElementow.Count > 0) //wyszukuje wszystkie Elementy do usunięcia
            {
                Element tempElement = kolejkaElementow.Dequeue();
                listaIdDoUsuniecia.Add(tempElement.id);
                if (tempElement.HasItems)
                {
                    foreach (Element o in tempElement.Items)
                    {
                        if (o is Element)
                            kolejkaElementow.Enqueue(o as Element);
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

            foreach (XmlNode node in nodeList)
            {
                if (listaIdDoUsuniecia.Contains(node.Attributes["Id"].InnerText))
                    nodesToRemove.Enqueue(node);
            }

            while (nodesToRemove.Count > 0)
            {
                XmlNode tempNode = nodesToRemove.Dequeue();
                tempNode.ParentNode.RemoveChild(tempNode);
            }

            xml.Save(Properties.Settings.Default.baseXmlPath);
        }

        protected static object getSenderParent(object sender)
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
    }
}
