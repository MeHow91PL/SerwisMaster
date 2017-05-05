using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerwisMaster
{
    interface IPolaczenieZBazaDanych
    {
        List<Element> pobierzElementy();
    }
}
