using SerwisMaster.DAL;
using SerwisMaster.Polaczenia;
using SerwisMaster.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerwisMaster.BL
{
    class BazaLocalDb : IBazaDanych
    {

        public bool DodajElement(Element element)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ElementModel elementDb = new ElementModel() { Klucz = element.Klucz, Nazwa = element.Nazwa, Opis = element.Opis, Rodzaj = element.Rodzaj, KluczRodzica = element.KluczRodzica };

            switch (element.Rodzaj)
            {
                case RodzajElementu.Folder:
                    db.Elementy.Add(elementDb);
                    break;
                case RodzajElementu.Klient:
                    KlientModel klient = element as Klient;
                    db.Klienci.Add(klient);
                    break;
                case RodzajElementu.Rdp:
                    RdpModel rdpModel = element as Rdp;
                    db.PolaczeniaRdp.Add(rdpModel);
                    break;
                case RodzajElementu.TeamViewer:
                    TeamViewerModel tvModel = element as TeamViewer;
                    db.PolaczeniaTeamViewer.Add(tvModel);
                    break;
                case RodzajElementu.WebBrowser:
                    WebBrowserModel webModel = element as WebBrowser;
                    db.PolaczeniaWebBrowser.Add(webModel);
                    break;
                case RodzajElementu.Inne:
                    break;
                default:
                    break;
            }


            db.SaveChanges();
            return true;
        }

        public Element PobierzElementPoKluczu(string klucz)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Element resultElement;
            if (db.Elementy.Any(e => e.Klucz == klucz))
            {
                ElementModel elementFromDB = db.Elementy.SingleOrDefault(k => k.Klucz == klucz);
                switch (elementFromDB.Rodzaj)
                {
                    case RodzajElementu.Folder:
                        resultElement = (Folder)elementFromDB;
                        break;
                    case RodzajElementu.Klient:
                        KlientModel klientModel = db.Klienci.SingleOrDefault(k => k.Element.Klucz == klucz);
                        resultElement = (Klient)klientModel;
                        break;
                    case RodzajElementu.Rdp:
                        RdpModel rdpModel = db.PolaczeniaRdp.SingleOrDefault(k => k.Element.Klucz == klucz);
                        resultElement = (Rdp)rdpModel;
                        break;
                    case RodzajElementu.TeamViewer:
                        TeamViewerModel teamViewerModel = db.PolaczeniaTeamViewer.SingleOrDefault(k => k.Element.Klucz == klucz);
                        resultElement = (TeamViewer)teamViewerModel;
                        break;
                    case RodzajElementu.WebBrowser:
                        WebBrowserModel webBrowserModel = db.PolaczeniaWebBrowser.SingleOrDefault(k => k.Element.Klucz == klucz);
                        resultElement = (WebBrowser)webBrowserModel;
                        break;
                    default:
                        resultElement = null;
                        throw new Exception("Błąd pobierania elementu");
                }
            }
            else
            {
                resultElement = null;
            }
            return resultElement;
        }

        public bool CzyElementJuzIstnieje(AtrybutElementu atrybut, Element element)
        {
            try
            {
                ApplicationDbContext db = new ApplicationDbContext();

                bool result;
                switch (atrybut)
                {
                    case AtrybutElementu.Nazwa:
                        result = db.Elementy.Any(e => e.Status == StatusElementu.Aktywny && e.Nazwa == element.Nazwa);
                        break;
                    case AtrybutElementu.Klucz:
                        result = db.Elementy.Any(e => e.Status == StatusElementu.Aktywny && e.Klucz == element.Klucz);
                        break;
                    case AtrybutElementu.KluczRodzica:
                        result = db.Elementy.Any(e => e.Status == StatusElementu.Aktywny && e.KluczRodzica == element.KluczRodzica);
                        break;
                    default:
                        result = false;
                        break;
                }
                return result;
            }
            catch (NullReferenceException)
            {
                MyMessageBox.Show("NullReference w sprawdzaniu Czy element istenije. Zwrócone będzie false i powinno działać.");
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Element> PobierzWszystkieElementy()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            Queue<Element> PobraneElementy = new Queue<Element>(); //kolejka do pobierania elementów podległych
            List<Element> ListaElementow = new List<Element>(); //gotowa lista zwracana do programu
            List<Element> ElementyNaLiscie = new List<Element>(); //lista wszystkich elementów dodanych już do listy elementów
            IQueryable<ElementModel> ElementyPobraneZBazy; //potrzbna, żeby później określić czy pobrane będą wszystkie czy tylko aktywne elementy
            string pobierzTylkoAktywne = PobierzWartoscOpcji(NazwaOpcji.PokazUkryteElementy);
            PobraneElementy.Clear();
            ListaElementow.Clear();
            ElementyNaLiscie.Clear();


            if (pobierzTylkoAktywne == "T")
            {
                ElementyPobraneZBazy = db.Elementy.Where(e => e.Status == StatusElementu.Aktywny);
            }
            else
            {
                ElementyPobraneZBazy = db.Elementy;
            }

            foreach (var item in ElementyPobraneZBazy)
            {
                Element elem;
                switch (item.Rodzaj)
                {
                    case RodzajElementu.Folder:
                        elem = new Folder(item.Nazwa, item.KluczRodzica, item.Opis, item.Klucz);
                        break;
                    case RodzajElementu.Klient:
                        KlientModel klientModel = db.Klienci.SingleOrDefault(k => k.Element.Klucz == item.Klucz);
                        elem = (Klient)klientModel;
                        break;
                    case RodzajElementu.Rdp:
                        RdpModel rdpModel = db.PolaczeniaRdp.SingleOrDefault(k => k.Element.Klucz == item.Klucz);
                        elem = (Rdp)rdpModel;
                        break;
                    case RodzajElementu.TeamViewer:
                        TeamViewerModel teamViewerModel = db.PolaczeniaTeamViewer.SingleOrDefault(k => k.Element.Klucz == item.Klucz);
                        elem = (TeamViewer)teamViewerModel;
                        break;
                    case RodzajElementu.WebBrowser:
                        WebBrowserModel webBrowserModel = db.PolaczeniaWebBrowser.SingleOrDefault(k => k.Element.Klucz == item.Klucz);
                        elem = (WebBrowser)webBrowserModel;
                        break;
                    default:
                        elem = null;
                        throw new Exception("Błąd pobierania elementu");
                }

                PobraneElementy.Enqueue(elem);
            }

            PobraneElementy.OrderBy(o => o.KluczRodzica);
            while (PobraneElementy.Count > 0)
            {
                Element element = PobraneElementy.Dequeue();
                if (String.IsNullOrWhiteSpace(element.KluczRodzica))//jeżeli element nie ma rodzica to dodaj bezpośrednio na listę elementów
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

        public void EdytujElement(Element element)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            ElementModel elementModel = db.Elementy.SingleOrDefault(e => e.Klucz == element.Klucz);

            switch (elementModel.Rodzaj)
            {
                case RodzajElementu.Folder:
                    elementModel = element;
                    db.Entry(elementModel).State = EntityState.Modified;
                    break;
                case RodzajElementu.Klient:
                    KlientModel klientModel = db.Klienci.SingleOrDefault(k => k.Element.Klucz == element.Klucz);
                    klientModel = (Klient)element;
                    db.Entry(klientModel).State = EntityState.Modified;
                    break;
                case RodzajElementu.Rdp:
                    RdpModel rdpModel = db.PolaczeniaRdp.SingleOrDefault(k => k.Element.Klucz == element.Klucz);
                    rdpModel = (Rdp)element;
                    db.Entry(rdpModel).State = EntityState.Modified;
                    break;
                case RodzajElementu.TeamViewer:
                    TeamViewerModel teamViewerModel = db.PolaczeniaTeamViewer.SingleOrDefault(k => k.Element.Klucz == element.Klucz);
                    teamViewerModel = (TeamViewer)element;
                    db.Entry(teamViewerModel).State = EntityState.Modified;
                    break;
                case RodzajElementu.WebBrowser:
                    WebBrowserModel webBrowserModel = db.PolaczeniaWebBrowser.SingleOrDefault(k => k.Element.Klucz == element.Klucz);
                    webBrowserModel = (WebBrowser)element;
                    db.Entry(webBrowserModel).State = EntityState.Modified;
                    break;
                case RodzajElementu.Inne:
                    break;
                default:
                    break;
            }

            db.SaveChanges();
        }


        public bool UkryjElement(Element element)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ElementModel elementModel = db.Elementy.SingleOrDefault(e => e.Klucz == element.Klucz);
            elementModel.Status = StatusElementu.Ukryty;
            db.Entry(elementModel).State = EntityState.Modified;
            db.SaveChanges();

            return true;
        }

        public bool UkryjElementy(IEnumerable<string> kluczeElementow)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            foreach (var kluczElementuDoUsuniecia in kluczeElementow)
            {
                using (ElementModel elementModel = db.Elementy.SingleOrDefault(e => e.Klucz == kluczElementuDoUsuniecia))
                {
                    elementModel.Status = StatusElementu.Ukryty;
                    db.Entry(elementModel).State = EntityState.Modified;
                }
            }
            db.SaveChanges();

            return true;
        }

        public bool UsunElement(Element element)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            db.Elementy.Remove(db.Elementy.Find(element.Klucz));
            db.SaveChanges();
            return true;
        }

        public bool UsunWszystkieElementy()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            foreach (var item in db.Elementy)
            {
                db.Elementy.Remove(item);
            }

            db.SaveChanges();
            return true;
        }

        public bool ZmienNazwe(Element element)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ElementModel elementModel = db.Elementy.Single(e => e.Klucz == element.Klucz);
            elementModel.Nazwa = element.Nazwa;
            db.SaveChanges();
            return true;
        }

        public void UstawOpcje(NazwaOpcji nazwa, string wartosc)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            switch (nazwa)
            {
                case NazwaOpcji.PokazUkryteElementy:
                    if (wartosc == "T" || wartosc == "N")
                    {
                        using (Opcje opcja = db.Opcje.SingleOrDefault(o => o.Nazwa == "PokazUkryteElementy"))
                        {
                            opcja.Wartosc = wartosc;
                            db.Entry(opcja).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        MyMessageBox.Show("Przekazano błędny argument dla opcji PokazUkryteElementy. Oczekiwana wartość to T lub N");
                        return;
                    }
                    break;
                default:
                    break;
            }

            db.SaveChanges();
        }

        public string PobierzWartoscOpcji(NazwaOpcji nazwa)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string nazwaOpcji = Enum.GetName(nazwa.GetType(), nazwa);
            string result = db.Opcje.SingleOrDefault(o => o.Nazwa == nazwaOpcji).Wartosc;
            return result;
        }

        public List<Element> WyszukajElementyPoNazwie(string szukanaNazwa, bool szukajWPodciagach)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Element> elementyPodbraneZBazy = PobierzWszystkieElementy();
            List<Element> result;

            if (szukajWPodciagach)
            {
                result = elementyPodbraneZBazy.Where(e => e.Nazwa.ToUpper().TrimStart().TrimEnd().Contains(szukanaNazwa.ToUpper())).ToList();
            }
            else
            {
                result = elementyPodbraneZBazy.Where(e => e.Nazwa.ToUpper().TrimStart().TrimEnd().StartsWith(szukanaNazwa.ToUpper())).ToList();
            }

            return result;
        }


    }

    public enum AtrybutElementu
    {
        Nazwa,
        Klucz,
        KluczRodzica
    }
}
