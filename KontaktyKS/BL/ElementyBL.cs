using SerwisMaster.DAL;
using SerwisMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerwisMaster.BL
{


    public abstract class ElementyBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //public List<Element> PobierzWszystkieElementy()
        //{
        //    Queue<Element> PobraneElementy = new Queue<Element>(); //kolejka do pobierania elementów podległych
        //    List<Element> ListaElementow = new List<Element>(); //gotowa lista zwracana do programu
        //    List<Element> ElementyNaLiscie = new List<Element>(); //lista wszystkich elementów dodanych już do listy elementów
        //    PobraneElementy.Clear();
        //    ListaElementow.Clear();
        //    ElementyNaLiscie.Clear();

        //    foreach (var item in db.Elementy)
        //    {
        //        Element elem;
        //        switch (item.Rodzaj)
        //        {
        //            case RodzajElementu.Folder:
        //                elem = new Folder(item.Nazwa, item.IdRodzica.ToString(), item.Opis, item.Id.ToString());
        //                break;
        //            case RodzajElementu.Klient:
        //                elem = new Klient(item.Nazwa, item.IdRodzica.ToString(), item.Opis, null, null, null, item.Id.ToString());
        //                break;
        //            case RodzajElementu.Inne:
        //                elem = new TeamViewer(item.Nazwa, item.IdRodzica.ToString(), item.Opis, "", "", "", item.Id.ToString());
        //                break;
        //            default:
        //                elem = null;
        //                throw new Exception("Błąd pobierania elementu");
        //        }

        //        PobraneElementy.Enqueue(elem);
        //    }


        //    while (PobraneElementy.Count > 0)
        //    {
        //        Element element = PobraneElementy.Dequeue();
        //        if (string.IsNullOrWhiteSpace(element.KluczRodzica))//jeżeli element nie ma rodzica to dodaj bezpośrednio na listę elementów
        //        {
        //            ListaElementow.Add(element);
        //            ElementyNaLiscie.Add(element);
        //        }
        //        else if (ElementyNaLiscie.Any(e => e.Id == element.KluczRodzica)) //jeżeli element na rodzica i jest on już na liście to dodaj element jako item
        //        {
        //            Element parent = ElementyNaLiscie.Single(e => e.Id == element.KluczRodzica);
        //            parent.Items.Add(element);
        //            ElementyNaLiscie.Add(element);
        //        }
        //        else//jeżeli ma rodzica, którego jeszcze nie ma na liście elementów dodaj go z powrotem na kolejkę
        //        {
        //            PobraneElementy.Enqueue(element);
        //        }
        //    }

        //    return ListaElementow;
        //}


        
    }
}

