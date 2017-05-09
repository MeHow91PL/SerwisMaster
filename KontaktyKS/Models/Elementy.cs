using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerwisMaster.Models
{
    [Table("Elementy")]
    public partial class Elementy
    {
        public int Id { get; set; }

        public RodzajElementu Rodzaj { get; set; }

        public string Nazwa { get; set; }

        public string Opis { get; set; }

        public int IdRodzica { get; set; }

        public virtual List<Elementy> Dzieci { get; set; }
    }

    public enum RodzajElementu
    {
        Folder,
        Klient,
        Inne
    }
}
