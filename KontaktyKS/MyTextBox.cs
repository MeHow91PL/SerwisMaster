using SerwisMaster.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace SerwisMaster
{
    class MyTextBox : TextBox
    {

        Regex patt = new Regex(@"^[-_0-9A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż]$");
        Folder folder = null;
        static IBazaDanych db = new BazaLocalDb();


        public MyTextBox(Folder folder)
        {
            this.folder = folder;
            this.KeyDown += txtBox_KeyDown;
            this.LostFocus += txtBox_LostFocus;
            //this.PreviewTextInput += MyTextBox_PreviewTextInput;
        }

        void MyTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!patt.IsMatch(e.Text))
                e.Handled = true;
        }


        void txtBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                ConfirmChangeName();
        }

        void txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfirmChangeName();
        }

        private void ConfirmChangeName()
        {
            folder.Nazwa = ((folder.Header as WrapPanel).Children[1] as TextBox).Text;
            db.ZmienNazwe(folder);
            MainWindow.aktualizujTreeView(MainWindow.listOfClients);
        }

        public enum TextboxEditStatus
        {
            Confirmed,
            Aborted
        }

        //private void ConfirmChangeNameXML()
        //{
        //    XmlDocument xml = new XmlDocument();
        //    xml.Load(Properties.Settings.Default.baseXmlPath);
        //    XmlNodeList nodelist = xml["Connections"].ChildNodes;

        //    foreach (XmlNode node in nodelist)
        //    {
        //        if (node.Attributes["Id"].InnerText == folder.Klucz)
        //            node.Attributes["Name"].InnerText = ((folder.Header as WrapPanel).Children[1] as TextBox).Text;
        //    }

        //    xml.Save(Properties.Settings.Default.baseXmlPath);

        //    MainWindow.aktualizujTreeView(MainWindow.listOfClients);
        //}
    }
}
