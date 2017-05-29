using SerwisMaster.Polaczenia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml;
using ImportFromRemoteDesktopManager;
using System.Collections;
using SerwisMaster.BL;
using SerwisMaster.Models;

namespace SerwisMaster
{
    class ImportRemoteDesktopManager
    {
        static int ilosPominietychElementow = 0;
        static int IloscPrzygotowanychDoImportu = 0;
        static int IloscPoprawnieZaimportowanych = 0;

        public static void ImportRdmToLocalDb(string databasePath)
        {
            try
            {
                IBazaDanych db = new BazaLocalDb();

                List<string> listaIdIstniejacychElementow = new List<string>();
                foreach (var item in db.PobierzWszystkieElementy())
                {
                    listaIdIstniejacychElementow.Add(item.Klucz);
                }

                List<GroupRDM> groupsRdmList = ImportOfElements.GetGroupsList(databasePath);
                List<Folder> groupsList = ConvGroupRdmToFolder(groupsRdmList, listaIdIstniejacychElementow);
                IloscPrzygotowanychDoImportu = groupsList.Count;

                foreach (var item in groupsList) //pętla testowa
                {
                    Folder element = new Folder() { Klucz = item.Klucz, KluczRodzica = item.KluczRodzica, Nazwa = item.Nazwa, Opis = item.Opis };
                    db.DodajElement(element);
                    IloscPoprawnieZaimportowanych++;
                }
                List<TeamViewerRDM> teamViewersRdmList = ImportOfElements.GetTeamViewersList(databasePath);
                List<TeamViewer> teamViewers = ConvertTeamViewerRdmToTeamViewer(teamViewersRdmList, groupsRdmList, listaIdIstniejacychElementow);
                IloscPrzygotowanychDoImportu += teamViewers.Count;

                List<RdpRDM> rdpRdmList = ImportOfElements.GetRdpList(databasePath);
                List<Rdp> rdpList = ConvertRdpRdmToRdp(rdpRdmList, groupsRdmList, listaIdIstniejacychElementow);
                IloscPrzygotowanychDoImportu += rdpList.Count;

                MyMessageBox.Show("Import kontaktów zakończony pomyślnie.\nIlość zaimportowanych elementów: " + IloscPoprawnieZaimportowanych +
                   "\nIlość błędnych elementów: " + (IloscPrzygotowanychDoImportu - IloscPoprawnieZaimportowanych) +
                    "\nIlość pominiętych elementów: " + ilosPominietychElementow, "Import zakończony", MyMessageBoxButtons.Ok);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public static void ImportRDM(string databasePath)
        //{
        //    try
        //    {
        //        XmlDocument xml = new XmlDocument();
        //        xml.Load(Properties.Settings.Default.baseXmlPath);
        //        XmlNodeList nodeList = xml["Connections"].ChildNodes;
        //        List<string> listaIdIstniejacychElementow = new List<string>();
        //        foreach (XmlNode node in nodeList)
        //        {
        //            listaIdIstniejacychElementow.Add(node.Attributes["Id"].InnerText);
        //        }

        //        List<GroupRDM> groupsRdmList = ImportOfElements.GetGroupsList(databasePath);
        //        List<Folder> groupsList = ConvGroupRdmToFolder(groupsRdmList, listaIdIstniejacychElementow);

        //        List<TeamViewerRDM> teamViewersRdmList = ImportOfElements.GetTeamViewersList(databasePath);
        //        List<TeamViewer> teamViewers = ConvertTeamViewerRdmToTeamViewer(teamViewersRdmList, groupsRdmList, listaIdIstniejacychElementow);

        //        List<RdpRDM> rdpRdmList = ImportOfElements.GetRdpList(databasePath);
        //        List<Rdp> rdpList = ConvertRdpRdmToRdp(rdpRdmList, groupsRdmList, listaIdIstniejacychElementow);


        //        foreach (Folder groupItem in groupsList)
        //        {
        //            Serializator.serializuj(groupItem);
        //            ilosZaimportowanychElementow++;
        //        }
        //        MyMessageBox.Show("Import kontaktów zakończony pomyślnie.\nIlość zaimportowanych elementów: " + ilosZaimportowanychElementow +
        //            "\nIlość pominiętych elementów: " + ilosPominietychElementow, "Import zakończony", MyMessageBoxButtons.Ok);

        //        MainWindow.aktualizujTreeView(MainWindow.listOfClients);
        //    }
        //    catch (Exception ex)
        //    {
        //        MyMessageBox.Show(ex.Message, "Błąd importu!", MyMessageBoxButtons.Ok);
        //    }

        //}

        private static List<Folder> ConvGroupRdmToFolder(List<GroupRDM> list, List<string> listaIdIstniejacychElementow)
        {
            List<Folder> fols = new List<Folder>();
            list.OrderBy(e => e.group);
            bool wyswietlPytanie = true;
            MessageBoxResult result = MessageBoxResult.POMIN;

            for (int i = 0; i < list.Count; i++) //pętla przegląda elementy, żeby sprawdzić czy pozycje importowane już istnieją w bazie
            {
                if (listaIdIstniejacychElementow.Any(a => a == list[i].id))
                {
                    if (wyswietlPytanie)
                    {
                        result = MyMessageBox.Show("Próbujesz zaimportować elementy, które już są w bazie danych. Co zrobić z tymi elementami?", "Znaleziono duplikaty", MyMessageBoxButtons.PominPopraw);
                        wyswietlPytanie = false;
                    }

                    if (result == MessageBoxResult.POMIN)
                    {
                        // MyMessageBox.Show("Istniejące elementy zostaną pominięte w impocie!");
                        list.Remove(list[i]);
                        ilosPominietychElementow++;
                    }
                    else if (result == MessageBoxResult.POPRAW)
                    {
                        // MyMessageBox.Show("Istniejące elementy zostaną podmienione na elementy z importowanej bazy!");
                    }
                }
            }

            foreach (GroupRDM fol in list)
            {
                string groupId = ""; //pole poszłuży do odnalezienia id rodzica na podstawie ciągu grupy z RDM

                if (fol.name != fol.group)
                {
                    string folGroup = "";
                    try
                    {
                        folGroup = fol.group.Replace("\\" + fol.name, "");
                        groupId = list.Single(f => f.group.ToUpper().TrimEnd() == folGroup.ToUpper().TrimEnd()).id;
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.Show(ex.Message, "Błąd importu!", MyMessageBoxButtons.Ok);
                        return new List<Folder>();
                    }
                }
                fols.Add(new Folder(fol.name, groupId, "", fol.id));
            }

            return fols;
        }

        private static string Wzor(string groupPath, List<GroupRDM> groupsList)
        {
            string id = "";

            foreach (GroupRDM item in groupsList)
            {
                if (item.name == groupPath)
                    id = item.id;
            }

            return id;
        }

        private static List<TeamViewer> ConvertTeamViewerRdmToTeamViewer(List<TeamViewerRDM> teamViewersRdmList, List<GroupRDM> groups, List<string> listaIdIstniejacychElementow)
        {
            IBazaDanych db = new BazaLocalDb();
            List<TeamViewer> list = new List<TeamViewer>();

            for (int i = 0; i < list.Count; i++)
            {
                if (listaIdIstniejacychElementow.Any(a => a == list[i].Klucz))
                {
                    list.Remove(list[i]);
                    ilosPominietychElementow++;
                }
            }

            foreach (TeamViewerRDM teamVerwerRdm in teamViewersRdmList)
            {
                string group = "";

                foreach (var item in groups)
                {
                    if (item.group.Length >= teamVerwerRdm.group.Length && item.group.Substring(0, teamVerwerRdm.group.Length) == teamVerwerRdm.group)
                    {
                        group = item.id;
                    }
                }

                TeamViewer tv = new TeamViewer(teamVerwerRdm.name, group, teamVerwerRdm.description, "", teamVerwerRdm.connectionType, teamVerwerRdm.teamViewerId, teamVerwerRdm.id);
                db.DodajElement(tv);
                IloscPoprawnieZaimportowanych++;
            }

            return list;
        }

        private static List<Rdp> ConvertRdpRdmToRdp(List<RdpRDM> rdpRdmList, List<GroupRDM> groups, List<string> listaIdIstniejacychElementow)
        {
            IBazaDanych db = new BazaLocalDb();
            List<Rdp> list = new List<Rdp>();

            for (int i = 0; i < list.Count; i++)
            {
                if (listaIdIstniejacychElementow.Any(a => a == list[i].Klucz))
                {
                    list.Remove(list[i]);
                    ilosPominietychElementow++;
                }
            }

            foreach (RdpRDM RdpRdm in rdpRdmList)
            {
                string group = "";

                foreach (var item in groups)
                {
                    if (item.group.Length >= RdpRdm.group.Length && item.group.Substring(0, RdpRdm.group.Length) == RdpRdm.group)
                    {
                        group = item.id;
                    }
                }
                Rdp rdp = new Rdp(RdpRdm.name, group, RdpRdm.description, "hasło", "Rdp", RdpRdm.url, "", RdpRdm.id);
                db.DodajElement(rdp);
                IloscPoprawnieZaimportowanych++;
            }
            return list;
        }

    }
}

