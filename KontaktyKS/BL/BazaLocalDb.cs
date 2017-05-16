using SerwisMaster.DAL;
using SerwisMaster.Klasy_połączenia;
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
        ApplicationDbContext db = new ApplicationDbContext();

        public bool DodajElement(Element element)
        {
            ElementModel elementDb = new ElementModel() { Klucz = element.Klucz, Nazwa = element.Nazwa, Opis = element.Opis, Rodzaj = element.Rodzaj, KluczRodzica = element.KluczRodzica };

            db.Elementy.Add(elementDb);
            db.SaveChanges();
            return true;
        }

        public List<Element> PobierzWszystkieElementy()
        {
            Queue<Element> PobraneElementy = new Queue<Element>(); //kolejka do pobierania elementów podległych
            List<Element> ListaElementow = new List<Element>(); //gotowa lista zwracana do programu
            List<Element> ElementyNaLiscie = new List<Element>(); //lista wszystkich elementów dodanych już do listy elementów
            PobraneElementy.Clear();
            ListaElementow.Clear();
            ElementyNaLiscie.Clear();

            foreach (var item in db.Elementy)
            {
                Element elem;
                switch (item.Rodzaj)
                {
                    case RodzajElementu.Folder:
                        elem = new Folder(item.Nazwa, item.KluczRodzica, item.Opis, item.Klucz);
                        break;
                    case RodzajElementu.Klient:
                        elem = new Klient(item.Nazwa, item.KluczRodzica, item.Opis, null, null, null, item.Klucz);
                        break;
                    case RodzajElementu.Rdp:
                        elem = new Rdp(item.Nazwa, item.KluczRodzica, item.Opis, "", "", "", "", item.Klucz);
                        break;
                    case RodzajElementu.TeamViewer:
                        elem = new TeamViewer(item.Nazwa, item.KluczRodzica, item.Opis, "", "", "", item.Klucz);
                        break;
                    case RodzajElementu.WebBrowser:
                        elem = new WebBrowser(item.Nazwa, item.KluczRodzica, item.Opis, "", "", item.Klucz);
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
                else if (ElementyNaLiscie.Any(e => e.Id == element.KluczRodzica)) //jeżeli element na rodzica i jest on już na liście to dodaj element jako item
                {
                    Element parent = ElementyNaLiscie.Single(e => e.Id == element.KluczRodzica);
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

        public bool UsunElement(Element element)
        {
            db.Elementy.Remove(db.Elementy.Find(element.Id));
            db.SaveChanges();
            return true;
        }

        public bool UsunWszystkieElementy()
        {
            foreach (var item in db.Elementy)
            {
                db.Elementy.Remove(item);
            }

            db.SaveChanges();
            return true;
        }

        //public List<Element> WyszukajElementyPoNazwie(string szukanaNazwa, bool szukajWPodciagach)
        //{
        //    this.PobierzWszystkieElementy();
        //}
    }
}
