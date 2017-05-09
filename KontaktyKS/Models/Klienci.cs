using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerwisMaster.Models
{
    public class Klienci
    {
        public int ID { get; set; }

        public string haslo { get; set; }

        public virtual Elementy Element{ get; set; }

    }
}
