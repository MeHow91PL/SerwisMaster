using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerwisMaster.Models
{
    [Table("Elementy")]
    public class ElementModel
    {
        public int Id { get; set; }

        public string Klucz { get; set; }

        public RodzajElementu Rodzaj { get; set; }

        public string Nazwa { get; set; }

        public string Opis { get; set; }

        public int ParentId { get; set; }

        public string KluczRodzica { get; set; }

        public virtual List<ElementModel> Dzieci { get; set; }
    }

    public enum RodzajElementu
    {
        Folder,
        Klient,
        Rdp,
        TeamViewer,
        WebBrowser,
        Inne
    }
}
