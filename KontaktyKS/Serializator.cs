using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.IO;
using KontaktyKS.Klasy_połączenia;

namespace KontaktyKS
{
    class Serializator
    {
        public static void serializuj(Folder obj, XmlNode RemoteConnection = null)
        {
            string connectionsPath = Properties.Settings.Default.baseXmlPath;
            XmlDocument xml = new XmlDocument();
            if (!File.Exists(connectionsPath))
            {
                createDataBaseFile(connectionsPath);
            }
            xml.Load(connectionsPath);
            XmlNode rootNode = xml["Connections"];

            XmlNode connectNode = xml.CreateElement("Connection");

            connectNode.Attributes.Append(xml.CreateAttribute("Type"));
            connectNode.Attributes.Append(xml.CreateAttribute("Name"));
            connectNode.Attributes.Append(xml.CreateAttribute("Group"));
            connectNode.Attributes.Append(xml.CreateAttribute("Id"));
            connectNode.Attributes["Id"].InnerText = obj.id.ToString();

            if (obj.parent is Folder)
                connectNode.Attributes["Group"].InnerText = (obj.parent as Folder).id.ToString();
            else if (obj.Parent is Folder)
                connectNode.Attributes["Group"].InnerText = (obj.Parent as Folder).id.ToString();
            else
                connectNode.Attributes["Group"].InnerText = "";


            if (obj is Klient)
            {
                Klient klient = (Klient)obj;

                connectNode.Attributes["Type"].InnerText = "Klient";
                connectNode.Attributes["Name"].InnerText = klient.nazwa;


                XmlNode emails = xml.CreateElement("Emails");

                for (int i = 0; i < klient.emailList.Count; i++)
                {
                    XmlNode email = xml.CreateElement("Email");
                    email.Attributes.Append(xml.CreateAttribute("Address"));
                    email.Attributes["Address"].InnerText = klient.emailList[i].adresEmail;
                    emails.AppendChild(email);
                }

                connectNode.AppendChild(emails);


                XmlNode phones = xml.CreateElement("Phones");

                for (int i = 0; i < klient.telefonList.Count; i++)
                {
                    XmlNode phone = xml.CreateElement("Phone");
                    phone.Attributes.Append(xml.CreateAttribute("Name"));
                    phone.Attributes.Append(xml.CreateAttribute("Number"));
                    phone.Attributes["Name"].InnerText = klient.telefonList[i].nazwa;
                    phone.Attributes["Number"].InnerText = klient.telefonList[i].numer;
                    phones.AppendChild(phone);
                }

                connectNode.AppendChild(phones);


                XmlNode credentials = xml.CreateElement("Credentials");

                for (int i = 0; i < klient.daneLogowaniaList.Count; i++)
                {
                    XmlNode credential = xml.CreateElement("Credential");
                    credential.Attributes.Append(xml.CreateAttribute("Login"));
                    credential.Attributes.Append(xml.CreateAttribute("Password"));
                    credential.Attributes.Append(xml.CreateAttribute("Type"));
                    credential.Attributes["Login"].InnerText = klient.daneLogowaniaList[i].login;
                    credential.Attributes["Password"].InnerText = klient.daneLogowaniaList[i].haslo;
                    credential.Attributes["Type"].InnerText = klient.daneLogowaniaList[i].system;
                    credentials.AppendChild(credential);
                }
                connectNode.AppendChild(credentials);

                if (RemoteConnection == null)
                    connectNode.AppendChild(xml.CreateElement("RemoteConnections"));
                else
                {
                    XmlNode importNode = xml.ImportNode(RemoteConnection,true);
                    connectNode.AppendChild(importNode);
                }
            }
            else // if obj is Folder
            {
                connectNode.Attributes["Type"].InnerText = "Folder";
                connectNode.Attributes["Name"].InnerText = obj.nazwa;
            }


            rootNode.AppendChild(connectNode);

            xml.Save(connectionsPath);

        }

        public static List<Folder> deserializuj(String file)
        {
            XmlDocument xml = new XmlDocument();
            List<Folder> foldery = new List<Folder>();
            Queue<Folder> queue = new Queue<Folder>();
            XmlNodeList nodeList;

            List<XmlNode> connectionsNodes = new List<XmlNode>();
            Queue<Folder> childrenNodes = new Queue<Folder>();

            List<Folder> elementyNaLiscie = new List<Folder>();

            if (!File.Exists(file))
            {
                createDataBaseFile(file);
            }

            xml.Load(file);

            if (xml.ChildNodes[0] != null)
            {
                nodeList = xml.ChildNodes[0].ChildNodes;
            }
            else
            {
                MyMessageBox.Show("Plik XML jest uszkodzony, brakuje głównego węzła Connections");
                return foldery;
            }



            foreach (XmlNode node in nodeList)
            {
                //try
                //{
                Folder folder = null;

                if (node.Attributes["Type"].InnerText == "Folder")
                {
                    folder = new Folder(node.Attributes["Name"].InnerText, null) { id = node.Attributes["Id"].InnerText, group = node.Attributes["Group"].InnerText };
                }

                else if (node.Attributes["Type"].InnerText == "Klient")
                {
                    List<Email> emailList = new List<Email>();
                    List<Telefon> telefonList = new List<Telefon>();
                    List<DaneLogowania> daneLogowaniaList = new List<DaneLogowania>();

                    if (node["Emails"] != null && node["Emails"].HasChildNodes)
                    {
                        foreach (XmlNode tempNode in node["Emails"].ChildNodes)
                            emailList.Add(new Email() { adresEmail = tempNode.Attributes["Address"].InnerText });
                    }

                    if (node["Phones"] != null && node["Phones"].HasChildNodes)
                    {
                        foreach (XmlNode tempNode in node["Phones"].ChildNodes)
                            telefonList.Add(new Telefon()
                            {
                                nazwa = tempNode.Attributes["Name"].InnerText,
                                numer = tempNode.Attributes["Number"].InnerText
                            });
                    }

                    if (node["Credentials"] != null && node["Credentials"].HasChildNodes)
                    {
                        foreach (XmlNode tempNode in node["Credentials"].ChildNodes)
                        {
                            daneLogowaniaList.Add(new DaneLogowania()
                            {
                                system = tempNode.Attributes["Type"].InnerText,
                                login = tempNode.Attributes["Login"].InnerText,
                                haslo = tempNode.Attributes["Password"].InnerText
                            });
                        }
                    }

                    folder = new Klient(node.Attributes["Name"].InnerText, emailList, telefonList, daneLogowaniaList, null) { id = node.Attributes["Id"].InnerText, group = node.Attributes["Group"].InnerText };

                    if (node["RemoteConnections"] != null && node["RemoteConnections"].HasChildNodes)
                    {
                        foreach (XmlNode tempNode in node["RemoteConnections"].ChildNodes)
                        {
                            if (tempNode.Name == "Teamviewer")
                                folder.Items.Add(new TeamViewer(
                                    tempNode.Attributes["Id"].InnerText,
                                    tempNode.Attributes["Name"].InnerText,
                                    tempNode.Attributes["TeamViewerId"].InnerText,
                                    tempNode.Attributes["Password"].InnerText,
                                    "Teamviewer"));
                            if (tempNode.Name == "RDP")
                                folder.Items.Add(new Rdp(
                                    tempNode.Attributes["Id"].InnerText,
                                    tempNode.Attributes["Name"].InnerText,
                                    tempNode.Attributes["Address"].InnerText,
                                    tempNode.Attributes["Login"].InnerText,
                                    tempNode.Attributes["Password"].InnerText,
                                    "RDP"));
                        }
                    }
                }


                if (string.IsNullOrWhiteSpace(folder.group))
                {
                    foldery.Add(folder);
                    elementyNaLiscie.Add(folder);
                }
                else
                {
                    childrenNodes.Enqueue(folder);
                }
            }

            while (childrenNodes.Count > 0)
            {
                Folder tempFolder = null;
                var fol = childrenNodes.Dequeue();

                if (elementyNaLiscie.Any(f => f.id == fol.group))
                {
                    tempFolder = elementyNaLiscie.Single(f => f.id == fol.group);
                    tempFolder.Items.Add(fol);
                    elementyNaLiscie.Add(fol);
                }
                else
                    childrenNodes.Enqueue(fol);
            }

            foldery = foldery.OrderBy(e => e.nazwa).ToList();
            return foldery;
        }




    

    private static void createDataBaseFile(string connectionsPath)
    {
        XmlDocument newXml = new XmlDocument();
        XmlNode root = newXml.CreateNode(XmlNodeType.Element, "Connections", "Main");
        newXml.AppendChild(root);
        newXml.Save(connectionsPath);

            
    }

    }
}

