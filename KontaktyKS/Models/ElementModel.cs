using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerwisMaster.Models
{
    [Table("Elementy")]
    public class ElementModel : IDisposable
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Klucz { get; set; }

        public RodzajElementu Rodzaj { get; set; }

        public string Nazwa { get; set; }

        public string Opis { get; set; }

        public StatusElementu Status { get; set; }

        public int ParentId { get; set; }

        public string KluczRodzica { get; set; }

        public virtual List<ElementModel> Dzieci { get; set; }

        public static implicit operator ElementModel(Element element)
        {
            ElementModel elementModel = new ElementModel
            {
                Klucz = element.Klucz,
                Nazwa = element.Nazwa,
                Opis = element.Opis,
                Rodzaj = element.Rodzaj,
                KluczRodzica = element.KluczRodzica
            };

            return elementModel;
        }

        private bool isDisposed = false;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    // tu zwalniamy zasoby zarządzane (standardowe klasy)
                    this.Dzieci = null;
                }
                // tu zwalniamy zasoby niezarządzane (np. strumienie, obiekty COM itp.)
            }
            this.isDisposed = true;
        }
        ~ElementModel()
        {
            this.Dispose(false);
        }
    }

    public enum StatusElementu
    {
        Aktywny,
        Ukryty
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
