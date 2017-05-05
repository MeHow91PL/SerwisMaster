using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows;
using System.Xml;
using System.Windows.Documents;

namespace SerwisMaster
{
    class EdytujKlienta : DaneKlienta
    {
        Klient klient = null;

        public EdytujKlienta( Klient klient )
            : base( klient )
        {
            this.klient = klient;
            nazwaTextBox.Text = klient.nazwa;
            emailList = klient.emailList;
            telefonList = klient.telefonList;
            daneLogowaniaList = klient.daneLogowaniaList;
            opisRichTextbox.Document.Blocks.Clear();
            opisRichTextbox.Document.Blocks.Add(new Paragraph(new Run(klient.opis)) { Margin = new Thickness(0) });
            dodajKlientaButton.Content = "Zapisz";
            Title = "Edytuj klienta";
            base.ustawZrodlaList();
        }

        protected override void zapiszKlienta()
        {
            if( string.IsNullOrWhiteSpace( nazwaTextBox.Text ) )
            {

                MessageBox.Show( "Pole nazwa nie może być puste!", "Uzupełnij nazwę",
                    MessageBoxButton.OK, MessageBoxImage.Error );
            }
            else
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(Properties.Settings.Default.baseXmlPath);

                XmlNodeList nodeList = xml["Connections"].ChildNodes;

                foreach(XmlNode node in nodeList)
                {
                    if(node.Attributes["Id"].InnerText == klient.id)
                    {
                        xml["Connections"].RemoveChild(node);
                    }
                }

                xml.Save(Properties.Settings.Default.baseXmlPath);

                
                this.klient.nazwa = nazwaTextBox.Text;
                this.klient.emailList = this.emailList;
                this.klient.telefonList = this.telefonList;
                this.klient.daneLogowaniaList = this.daneLogowaniaList;
                this.klient.opis = new TextRange(opisRichTextbox.Document.ContentStart, opisRichTextbox.Document.ContentEnd).Text;
                Serializator.serializuj(this.klient);

                MainWindow.aktualizujTreeView(MainWindow.listOfClients);
                this.Close();

            }
        }
    }
}
