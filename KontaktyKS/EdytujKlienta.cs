using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows;
using System.Xml;


namespace KontaktyKS
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
                XmlNode remoteConnections = null;

                XmlNodeList nodeList = xml["Connections"].ChildNodes;

                foreach(XmlNode node in nodeList)
                {
                    if(node.Attributes["Id"].InnerText == klient.id)
                    {
                        remoteConnections = node.ChildNodes[3];
                        xml["Connections"].RemoveChild(node);
                    }
                }

                xml.Save(Properties.Settings.Default.baseXmlPath);

                
                this.klient.nazwa = nazwaTextBox.Text;
                this.klient.emailList = this.emailList;
                this.klient.telefonList = this.telefonList;
                this.klient.daneLogowaniaList = this.daneLogowaniaList;
                Serializator.serializuj(this.klient, remoteConnections);

                MainWindow.aktualizujTreeView(MainWindow.listOfClients);
                this.Close();

            }
        }
    }
}
