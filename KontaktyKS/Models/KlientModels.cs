using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SerwisMaster.Models
{
    [Table("Klienci")]
    public class KlientModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual ElementModel Element { get; set; }

        public virtual List<EmailModel> AdresyEmail { get; set; }
        public virtual List<TelefonModel> Telefony { get; set; }
        public virtual List<DaneLogowaniaModel> DaneLogowania { get; set; }

        public static implicit operator KlientModel(Klient klient)
        {
            ElementModel elementModel = new ElementModel
            {
                Klucz = klient.Klucz,
                Nazwa = klient.Nazwa,
                Opis = klient.Opis,
                Rodzaj = klient.Rodzaj,
                KluczRodzica = klient.KluczRodzica
            };
             
            KlientModel klientModel = new KlientModel()
            {
                Element = elementModel,
                AdresyEmail = klient.emailList,
                DaneLogowania = klient.daneLogowaniaList,
                Telefony = klient.telefonList
            };

            return klientModel;
        }

    }

    [Table("AdresyEmail")]
    public class EmailModel
    {
        [Key]
        public int Id { get; set; }

        public string AdresEmail { get; set; }

        public virtual KlientModel Klient { get; set; }
    }

    [Table("Telefony")]
    public class TelefonModel
    {
        [Key]
        public int Id { get; set; }

        public string Nazwa { get; set; }

        public string NumerTelefonu { get; set; }

        public virtual KlientModel Klient { get; set; }
    }

    [Table("DaneLogowania")]
    public class DaneLogowaniaModel
    {
        [Key]
        public int Id { get; set; }

        public string Login { get; set; }
        public string Haslo { get; set; }
        public RodzajSystemu System { get; set; }

        public virtual KlientModel Klient { get; set; }
    }

    public enum RodzajSystemu
    {
        Windows,
        KSPPS,
        KSSOMED,
        SZOI
    }
}
