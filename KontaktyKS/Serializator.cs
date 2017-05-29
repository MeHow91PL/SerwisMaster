using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.IO;
using SerwisMaster.Polaczenia;

namespace SerwisMaster
{
    class Serializator
    {
        //public static void serializuj(Element obj)
        //{
        //    try
        //    {
        //        string connectionsPath = Properties.Settings.Default.baseXmlPath;
        //        XmlDocument xml = new XmlDocument();

        //        xml.Load(connectionsPath);
        //        XmlNode rootNode = xml["Connections"];

        //        XmlNode connectNode = xml.CreateElement(obj.GetType().Name);

        //        connectNode.Attributes.Append(xml.CreateAttribute("Id"));
        //        connectNode.Attributes.Append(xml.CreateAttribute("Group"));
        //        connectNode.Attributes.Append(xml.CreateAttribute("Name"));
        //        connectNode.Attributes.Append(xml.CreateAttribute("Type"));
        //        connectNode.Attributes.Append(xml.CreateAttribute("Description"));

        //        connectNode.Attributes["Id"].InnerText = obj.Klucz.ToString();

        //        if (obj.parent is Folder)
        //            connectNode.Attributes["Group"].InnerText = (obj.parent as Folder).Id.ToString();
        //        else if (!string.IsNullOrWhiteSpace(obj.KluczRodzica))
        //            connectNode.Attributes["Group"].InnerText = obj.KluczRodzica;
        //        else
        //            connectNode.Attributes["Group"].InnerText = "";

        //        connectNode.Attributes["Description"].InnerText = obj.Opis;

        //        if (obj is Klient)
        //        {
        //            Klient klient = (Klient)obj;

        //            connectNode.Attributes["Type"].InnerText = "Klient";
        //            connectNode.Attributes["Name"].InnerText = klient.Nazwa;


        //            XmlNode emails = xml.CreateElement("Emails");

        //            for (int i = 0; i < klient.emailList.Count; i++)
        //            {
        //                XmlNode email = xml.CreateElement("Email");
        //                email.Attributes.Append(xml.CreateAttribute("Address"));
        //                email.Attributes["Address"].InnerText = klient.emailList[i].adresEmail;
        //                emails.AppendChild(email);
        //            }

        //            connectNode.AppendChild(emails);


        //            XmlNode phones = xml.CreateElement("Phones");

        //            for (int i = 0; i < klient.telefonList.Count; i++)
        //            {
        //                XmlNode phone = xml.CreateElement("Phone");
        //                phone.Attributes.Append(xml.CreateAttribute("Name"));
        //                phone.Attributes.Append(xml.CreateAttribute("Number"));
        //                phone.Attributes["Name"].InnerText = klient.telefonList[i].nazwa;
        //                phone.Attributes["Number"].InnerText = klient.telefonList[i].numer;
        //                phones.AppendChild(phone);
        //            }

        //            connectNode.AppendChild(phones);


        //            XmlNode credentials = xml.CreateElement("Credentials");

        //            for (int i = 0; i < klient.daneLogowaniaList.Count; i++)
        //            {
        //                XmlNode credential = xml.CreateElement("Credential");
        //                credential.Attributes.Append(xml.CreateAttribute("Login"));
        //                credential.Attributes.Append(xml.CreateAttribute("Password"));
        //                credential.Attributes.Append(xml.CreateAttribute("Type"));
        //                credential.Attributes["Login"].InnerText = klient.daneLogowaniaList[i].login;
        //                credential.Attributes["Password"].InnerText = klient.daneLogowaniaList[i].haslo;
        //                credential.Attributes["Type"].InnerText = klient.daneLogowaniaList[i].system;
        //                credentials.AppendChild(credential);
        //            }
        //            connectNode.AppendChild(credentials);
        //        }
        //        else if (obj is Folder) // if obj is Folder
        //        {
        //            connectNode.Attributes["Type"].InnerText = "Folder";
        //            connectNode.Attributes["Name"].InnerText = obj.Nazwa;
        //        }
        //        else if (obj is TeamViewer)
        //        {
        //            TeamViewer tv = obj as TeamViewer;
        //            connectNode.Attributes.Append(xml.CreateAttribute("TeamViewerId"));
        //            connectNode.Attributes.Append(xml.CreateAttribute("Password"));

        //            connectNode.Attributes["Name"].InnerText = tv.Nazwa;
        //            connectNode.Attributes["Type"].InnerText = "TeamViewer";
        //            connectNode.Attributes["TeamViewerId"].InnerText = tv.teamViewerId;
        //            if (string.IsNullOrWhiteSpace(tv.haslo)) tv.haslo = Properties.Settings.Default.defaultPasswordOfTeamViewer;
        //            connectNode.Attributes["Password"].InnerText = tv.haslo;
        //        }
        //        else if (obj is Rdp)
        //        {
        //            Rdp rdp = obj as Rdp;
        //            connectNode.Attributes.Append(xml.CreateAttribute("Address"));
        //            connectNode.Attributes.Append(xml.CreateAttribute("Login"));
        //            connectNode.Attributes.Append(xml.CreateAttribute("Password"));

        //            connectNode.Attributes["Name"].InnerText = rdp.Nazwa;
        //            connectNode.Attributes["Type"].InnerText = "Rdp";
        //            connectNode.Attributes["Address"].InnerText = rdp.adresRDP;
        //            connectNode.Attributes["Login"].InnerText = rdp.login;
        //            connectNode.Attributes["Password"].InnerText = rdp.haslo;
        //        }


        //        rootNode.AppendChild(connectNode);

        //        xml.Save(connectionsPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        MyMessageBox.Show("Treść błędu: \n" + ex.Message, "Wystąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
        //    }
        //}

        //public static List<Element> deserializuj(String file)
        //{
        //    try
        //    {
        //        XmlDocument xml = new XmlDocument();
        //        List<Element> elementy = new List<Element>();
        //        Queue<Element> queue = new Queue<Element>();
        //        XmlNodeList nodeList;

        //        List<XmlNode> connectionsNodes = new List<XmlNode>();
        //        Queue<Element> childrenNodes = new Queue<Element>();

        //        List<Element> elementyNaLiscie = new List<Element>();

        //        //while (true)
        //        //{

        //        //    try
        //        //    {
        //        //        xml.Load(file);

        //        //        break;
        //        //    }
        //        //    catch (Exception)
        //        //    {
        //        //    }

        //        //}

        //        if (xml.ChildNodes[0] != null)
        //        {
        //            nodeList = xml.ChildNodes[0].ChildNodes;
        //        }
        //        else
        //        {
        //            MyMessageBox.Show("Plik XML jest uszkodzony, brakuje głównego węzła Connections");
        //            return elementy;
        //        }

        //        foreach (XmlNode node in nodeList)
        //        {
        //            Element element = null;

        //            switch (node.Attributes["Type"].InnerText)
        //            {
        //                case "Folder":
        //                    deserializujFolder(node, out element);
        //                    break;
        //                case "Klient":
        //                    deserializujKlienta(node, out element);
        //                    break;
        //                case "TeamViewer":
        //                    deserializujTeamViewer(node, out element);
        //                    break;
        //                case "Rdp":
        //                    deserializujRdp(node, out element);
        //                    break;
        //                case "WebBrowser":
        //                    deserializujWebBrowser(node, out element);
        //                    break;
        //                default:
        //                    break;
        //            }

        //            if (string.IsNullOrWhiteSpace(element.KluczRodzica))
        //            {
        //                elementy.Add(element);
        //                elementyNaLiscie.Add(element);
        //            }
        //            else
        //            {
        //                childrenNodes.Enqueue(element);
        //            }
        //        }

        //        while (childrenNodes.Count > 0)
        //        {
        //            Element tempElement = null;
        //            var fol = childrenNodes.Dequeue();

        //            if (elementyNaLiscie.Any(f => f.Klucz == fol.KluczRodzica))
        //            {
        //                tempElement = elementyNaLiscie.Single(f => f.Klucz == fol.KluczRodzica);
        //                tempElement.Items.Add(fol);
        //                elementyNaLiscie.Add(fol);
        //            }
        //            else
        //                childrenNodes.Enqueue(fol);
        //        }

        //        elementy = elementy.OrderBy(e => e.Nazwa).ToList();
        //        return elementy;
        //    }
        //    catch(Exception ex)
        //    {
        //        MyMessageBox.Show("Treść błędu: \n" + ex.Message, "Wystąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
        //        return null;
        //    }
        //}


        //private static void deserializujFolder(XmlNode node, out Element element)
        //{
        //    element = new Folder(node.Attributes["Name"].InnerText, node.Attributes["Group"].InnerText, node.Attributes["Description"].InnerText, node.Attributes["Id"].InnerText, null);
        //}

        //private static void deserializujKlienta(XmlNode node, out Element element)
        //{
        //    try
        //    {
        //        List<EmailModel> emailList = new List<EmailModel>();
        //        List<Telefon> telefonList = new List<Telefon>();
        //        List<DaneLogowaniaModel> daneLogowaniaList = new List<DaneLogowaniaModel>();

        //        if (node["Emails"] != null && node["Emails"].HasChildNodes)
        //        {
        //            foreach (XmlNode tempNode in node["Emails"].ChildNodes)
        //                emailList.Add(new Email() { adresEmail = tempNode.Attributes["Address"].InnerText });
        //        }

        //        if (node["Phones"] != null && node["Phones"].HasChildNodes)
        //        {
        //            foreach (XmlNode tempNode in node["Phones"].ChildNodes)
        //                telefonList.Add(new Telefon()
        //                {
        //                    nazwa = tempNode.Attributes["Name"].InnerText,
        //                    numer = tempNode.Attributes["Number"].InnerText
        //                });
        //        }

        //        if (node["Credentials"] != null && node["Credentials"].HasChildNodes)
        //        {
        //            foreach (XmlNode tempNode in node["Credentials"].ChildNodes)
        //            {
        //                daneLogowaniaList.Add(new DaneLogowania()
        //                {
        //                    system = tempNode.Attributes["Type"].InnerText,
        //                    login = tempNode.Attributes["Login"].InnerText,
        //                    haslo = tempNode.Attributes["Password"].InnerText
        //                });
        //            }
        //        }

        //        element = new Klient(node.Attributes["Name"].InnerText, node.Attributes["Group"].InnerText, node.Attributes["Description"].InnerText, emailList, telefonList, daneLogowaniaList, node.Attributes["Id"].InnerText, null);

        //    }
        //    catch (Exception ex)
        //    {
        //        MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
        //        element = null;
        //    }
        //}

        //private static void deserializujTeamViewer(XmlNode node, out Element element)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(node.Attributes["Password"].InnerText))
        //            node.Attributes["Password"].InnerText = Properties.Settings.Default.defaultPasswordOfTeamViewer;

        //        element = new TeamViewer(node.Attributes["Name"].InnerText, node.Attributes["Group"].InnerText, node.Attributes["Description"].InnerText, node.Attributes["Password"].InnerText, node.Attributes["Type"].InnerText, node.Attributes["TeamViewerId"].InnerText, node.Attributes["Id"].InnerText);

        //    }
        //    catch (Exception ex)
        //    {
        //        MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
        //        element = null;
        //    }
        //}

        //private static void deserializujRdp(XmlNode node, out Element element)
        //{
        //    element = new Rdp(node.Attributes["Name"].InnerText, node.Attributes["Group"].InnerText, node.Attributes["Description"].InnerText, node.Attributes["Password"].InnerText, node.Attributes["Type"].InnerText, node.Attributes["Address"].InnerText, node.Attributes["Login"].InnerText, node.Attributes["Id"].InnerText);
        //}

        //private static void deserializujWebBrowser(XmlNode node, out Element element)
        //{
        //    throw new NotImplementedException();
        //}

        

    }
}

