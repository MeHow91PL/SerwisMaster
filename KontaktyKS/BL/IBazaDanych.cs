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
        //List<Element> WyszukajElementyPoNazwie(string szukanaNazwa, bool szukajWPodciagach);
    }
}
