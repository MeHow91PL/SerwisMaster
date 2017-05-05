using SerwisMaster.Klasy_połączenia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml;
using ImportFromRemoteDesktopManager;
using System.Collections;

namespace SerwisMaster
{
    class ImportRemoteDesktopManager
    {
        static uint ilosPominietychElementow = 0;
        static uint ilosZaimportowanychElementow = 0;

        public static void ImportRDM(string databasePath)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(Properties.Settings.Default.baseXmlPath);
                XmlNodeList nodeList = xml["Connections"].ChildNodes;
                List<string> listaIdIstniejacychElementow = new List<string>();
                foreach (XmlNode node in nodeList)
                {
                    listaIdIstniejacychElementow.Add(node.Attributes["Id"].InnerText);
                }

                List<GroupRDM> groupsRdmList = ImportOfElements.GetGroupsList(databasePath);
                List<Folder> groupsList = ConvGroupRdmToFolder(groupsRdmList, listaIdIstniejacychElementow);

                List<TeamViewerRDM> teamViewersRdmList = ImportOfElements.GetTeamViewersList(databasePath);
                List<TeamViewer> teamViewers = ConvertTeamViewerRdmToTeamViewer(teamViewersRdmList, groupsRdmList, listaIdIstniejacychElementow);

                List<RdpRDM> rdpRdmList = ImportOfElements.GetRdpList(databasePath);
                List<Rdp> rdpList = ConvertRdpRdmToRdp(rdpRdmList, groupsRdmList, listaIdIstniejacychElementow);


                foreach (Folder groupItem in groupsList)
                {
                    Serializator.serializuj(groupItem);
                    ilosZaimportowanychElementow++;
                }
                MyMessageBox.Show("Import kontaktów zakończony pomyślnie.\nIlość zaimportowanych elementów: " + ilosZaimportowanychElementow +
                    "\nIlość pominiętych elementów: " + ilosPominietychElementow, "Import zakończony",MyMessageBoxButtons.Ok);

                MainWindow.aktualizujTreeView(MainWindow.listOfClients);
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message, "Błąd importu!", MyMessageBoxButtons.Ok);
            }

        }

        private static List<Folder> ConvGroupRdmToFolder(List<GroupRDM> list, List<string> listaIdIstniejacychElementow)
        {
            List<Folder> fols = new List<Folder>();
            list.OrderBy(e => e.group);

            for(int i = 0; i < list.Count; i++)
            {
                if(listaIdIstniejacychElementow.Any(a => a == list[i].id))
                {
                    list.Remove(list[i]);
                    ilosPominietychElementow++;     
                }
            }

            foreach (GroupRDM fol in list)
            {
                string groupId = "";

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
                fols.Add(new Folder(fol.name,fol.group, "", fol.id, null));
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
            List<TeamViewer> list= new List<TeamViewer>();

            for (int i = 0; i < list.Count; i++)
            {
                if (listaIdIstniejacychElementow.Any(a => a == list[i].id))
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
                    if(item.group.Length >= teamVerwerRdm.group.Length && item.group.Substring(0, teamVerwerRdm.group.Length) == teamVerwerRdm.group)
                    {
                        group = item.id;
                    }
                }

                Serializator.serializuj(new TeamViewer(teamVerwerRdm.name, teamVerwerRdm.group, teamVerwerRdm.description, "", teamVerwerRdm.connectionType, teamVerwerRdm.teamViewerId));
                ilosZaimportowanychElementow++;
            }

            return list;
        }

        private static List<Rdp> ConvertRdpRdmToRdp(List<RdpRDM> rdpRdmList, List<GroupRDM> groups, List<string> listaIdIstniejacychElementow) 
        {
            List<Rdp> list= new List<Rdp>();

            for (int i = 0; i < list.Count; i++)
            {
                if (listaIdIstniejacychElementow.Any(a => a == list[i].id))
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
                Serializator.serializuj(new Rdp(RdpRdm.name, group, RdpRdm.description, "hasło", "Rdp", RdpRdm.url, "", RdpRdm.id));
                ilosZaimportowanychElementow++;
            }
            return list;
        }

    }
}

