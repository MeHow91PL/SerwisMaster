using SerwisMaster.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerwisMaster.BL
{
    interface IBazaDanych
    {
        List<Element> PobierzWszystkieElementy();
        Element PobierzElementPoKluczu(string klucz);

        /// <summary>
        /// Funkcja sprawdza czy dany element już istnieje w bazie danych
        /// </summary>
        /// <param name="atrybut">Według jakiego parametu funkcja ma sprawdzić czy element już istnieje</param>
        /// <param name="element">Element, który będzie poddany sprawdzeniu</param>
        /// <returns>Jeżeli element już istnieje fukcja zwróci true. Jeżeli nie istnieje funkcja zwróci false</returns>
        bool CzyElementJuzIstnieje(AtrybutElementu atrybut, Element element);

        bool DodajElement(Element element);
        void EdytujElement(Element element);
        bool ZmienNazwe(Element element);
        bool UsunElement(Element element);
        bool UkryjElement(Element element);
        bool UkryjElementy(IEnumerable<string> kluczeElementow);
        bool UsunWszystkieElementy();
        List<Element> WyszukajElementyPoNazwie(string szukanaNazwa, bool szukajWPodciagach);

        void UstawOpcje(NazwaOpcji nazwa, string wartosc);
        string PobierzWartoscOpcji(NazwaOpcji nazwa);
    }

    public enum NazwaOpcji
    {
        PokazUkryteElementy
    }

}
