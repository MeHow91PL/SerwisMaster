using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerwisMaster.Models
{   
    [Table("Opcje")]
    public class Opcje : IDisposable
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Wartosc { get; set; }

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
                }
                // tu zwalniamy zasoby niezarządzane (np. strumienie, obiekty COM itp.)
            }
            this.isDisposed = true;
        }
        ~Opcje()
        {
            this.Dispose(false);
        }
    }
}
