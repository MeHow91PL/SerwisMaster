using System;
using System.Windows.Input;
using System.Windows;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace SerwisMaster
{
    class TeamViewer : Polaczenie
    {
        public string teamViewerId = "";

        public TeamViewer(string nazwa, string group, string opis, string haslo, string typ, string teamViewerId, string id="", object parent=null)
            : base(nazwa, group, opis, haslo, typ, id, parent)
        {
            this.Rodzaj = Models.RodzajElementu.TeamViewer;
            this.teamViewerId = teamViewerId;
            Header = CreateHeader.createItemHeader(this);
            this.ToolTip = new ToolTip() { Content = this.Nazwa + "\nID: "+ this.teamViewerId + "\nHasło: " + haslo};
        }

        public override void uruchomPolaczenie(object sender, MouseButtonEventArgs e)
        {
            
            RegistryKey reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\TeamViewerConfiguration\DefaultIcon");

            string Wzor = (string)reg.GetValue("");
            string[] t = Wzor.Split('"');
            string path = t[1];

            if (!string.IsNullOrEmpty(path))
                System.Diagnostics.Process.Start(path, "-i " + teamViewerId +
                    " --Password " + base.haslo);
        }

        protected override void usunPolaczenie(object remoteId)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Properties.Settings.Default.baseXmlPath);
            XmlNodeList tv = xmlDoc.GetElementsByTagName("TeamViewer");

            foreach (XmlNode node in tv)
            {
                if (node.Attributes["Id"].InnerText == remoteId.ToString())
                {
                    node.ParentNode.RemoveChild(node);
                    xmlDoc.Save(Properties.Settings.Default.baseXmlPath);
                    break;
                }
            }
            
            
        }


    }
}
