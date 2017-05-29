using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Data;
using SerwisMaster.Polaczenia;

namespace SerwisMaster
{
    class PolaczeniePostgres : IPolaczenieZBazaDanych
    {
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        public static NpgsqlConnection OtworzPolaczenie(string server, string port, string userId, string password, string database )
        {
            string connStr = "Server=" + server + ";Port=" + port + ";UserId=" + userId + ";Password=" +
                password + ";Database=" + database + ";";
            NpgsqlConnection conn = new NpgsqlConnection(connStr);
            conn.Open();

            return conn;
        }

        public List<Element> pobierzElementy()
        {
            Queue<Element> PobraneElementy = new Queue<Element>(); //kolejka do pobierania elementów podległych
            List<Element> ListaElementow = new List<Element>(); //gotowa lista zwracana do programu
            List<Element> ElementyNaLiscie = new List<Element>(); //lista wszystkich elementów dodanych już do listy elementów
            PobraneElementy.Clear();
            ListaElementow.Clear();
            ElementyNaLiscie.Clear();

            NpgsqlConnection conn = OtworzPolaczenie("localhost", "5432", "postgres", "postgres", "SerwisMaster");
            string sql = "Select * from elem";
            //NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            //ds.Reset();
            //da.Fill(ds);
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                switch (dr["ntyp"].ToString())
                {
                    case "1":
                        PobraneElementy.Enqueue(new Klient(dr["nazw"].ToString(), dr["nrdzc"].ToString(), dr["opis"].ToString(), null, null, null, dr["nelem"].ToString()));
                        break;
                    case "2":
                        PobraneElementy.Enqueue(new Folder(dr["nazw"].ToString(), dr["nrdzc"].ToString(), dr["opis"].ToString(), dr["nelem"].ToString()));
                        break;
                    case "3":
                        PobraneElementy.Enqueue(new TeamViewer(dr["nazw"].ToString(), dr["nrdzc"].ToString(), dr["opis"].ToString(),dr["hasl"].ToString(), "TeamViewer", dr["adrs"].ToString() ,dr["nelem"].ToString()));
                        break;
                    case "4":
                        PobraneElementy.Enqueue(new Rdp(dr["nazw"].ToString(), dr["nrdzc"].ToString(), dr["opis"].ToString(), dr["hasl"].ToString(), "Rdp", dr["adrs"].ToString(), dr["logn"].ToString(), dr["nelem"].ToString()));
                        break;
                        //case "5":
                        //    listaElementow.Enqueue(new WebBrowser(dr["nazw"].ToString(), dr["nrdzc"].ToString(), dr["opis"].ToString(), dr["nelem"].ToString()));
                        //    break;
                }
            }

            while (PobraneElementy.Count > 0)
            {
                Element element = PobraneElementy.Dequeue();
                if (string.IsNullOrWhiteSpace(element.KluczRodzica))//jeżeli element nie ma rodzica to dodaj bezpośrednio na listę elementów
                {
                    ListaElementow.Add(element);
                    ElementyNaLiscie.Add(element);
                }
                else if (ElementyNaLiscie.Any(e => e.Klucz == element.KluczRodzica)) //jeżeli element na rodzica i jest on już na liście to dodaj element jako item
                {
                    Element parent = ElementyNaLiscie.Single(e => e.Klucz == element.KluczRodzica);
                    parent.Items.Add(element);
                    ElementyNaLiscie.Add(element);
                }
                else//jeżeli ma rodzica, którego jeszcze nie ma na liście elementów dodaj go z powrotem na kolejkę
                {
                    PobraneElementy.Enqueue(element);
                }
            }

            return ListaElementow;
        }
    }
}
