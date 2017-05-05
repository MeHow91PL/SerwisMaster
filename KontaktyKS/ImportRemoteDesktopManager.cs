using KontaktyKS.Klasy_połączenia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml;

namespace KontaktyKS
{
    class ImportRemoteDesktopManager
    {
        public static void importRDM(string xmlPath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlPath);

            XmlNodeList nodeList = xml.ChildNodes[1].ChildNodes;
            Queue<XmlNode> sortedNodeQueue = new Queue<XmlNode>();


            for (int i = 0; i < nodeList.Count; i++)
            {
                foreach (XmlNode node in nodeList)
                {
                    if (node["Group"] != null)
                    {
                        string[] s = node["Group"].InnerText.Split('\\');
                        if (s.Length == i)
                        {
                            sortedNodeQueue.Enqueue(node);
                        }
                    }
                    else if(i == 0)
                    {
                        sortedNodeQueue.Enqueue(node);
                    }
                    if (sortedNodeQueue.Count >= nodeList.Count)
                    {
                        goto toJuzWszystkie;
                    }
                }
            }
            toJuzWszystkie:

            while(sortedNodeQueue.Count > 0)
            {
                XmlNode tempNode = sortedNodeQueue.Dequeue();
                switch(tempNode["ConnectionType"].InnerText)
                {
                    case "TeamViewer":
                        importTeamviewer(tempNode);
                        break;
                    case "RDPConfigured":
                        importRDP(tempNode);
                        break;
                    case "Group":
                        importFolderu(tempNode);
                        break;
                        
                }
            }
        }



        private static void importTeamviewer(XmlNode node)
        {
        }

        private static void importRDP(XmlNode node)
        {
        }

        private static void importFolderu(XmlNode node)
        {
            Folder folder = new Folder();
            if(node["Group"].InnerText == node["Name"].InnerText)
            {
                folder.id = Guid.NewGuid().ToString();
                folder.nazwa = node["Name"].InnerText;
                Serializator.serializuj(folder);
            }
        }

    }
}

