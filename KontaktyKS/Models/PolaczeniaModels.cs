using SerwisMaster.Polaczenia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerwisMaster.Models
{
    [Table("PolaczeniaTeamViewer")]
    public class TeamViewerModel
    {
        public int Id { get; set; }
        public string TeamViewerId { get; set; }
        public string Haslo { get; set; }

        public virtual ElementModel Element { get; set; }

        public static implicit operator TeamViewerModel(TeamViewer tv)
        {
            ElementModel element = new ElementModel()
            {
                Klucz = tv.Klucz,
                KluczRodzica = tv.KluczRodzica,
                Nazwa = tv.Nazwa,
                Opis = tv.Opis,
                Rodzaj = tv.Rodzaj,
                Status = tv.Status
            };
            return new TeamViewerModel { TeamViewerId = tv.teamViewerId, Haslo = tv.haslo, Element = element };
        }
    }
    [Table("PolaczeniaRdp")]
    public class RdpModel
    {
        public int Id { get; set; }
        public string AdresRdp { get; set; }
        public string Login { get; set; }
        public string Haslo { get; set; }
        public virtual ElementModel Element { get; set; }

        public static implicit operator RdpModel(Rdp rdp)
        {
            ElementModel elementModel = new ElementModel
            {
                Klucz = rdp.Klucz,
                Nazwa = rdp.Nazwa,
                Opis = rdp.Opis,
                Rodzaj = rdp.Rodzaj,
                KluczRodzica = rdp.KluczRodzica
            };

            RdpModel rdpModel = new RdpModel()
            {
                Element = elementModel,
                AdresRdp = rdp.adresRDP,
                Login = rdp.login,
                Haslo = rdp.haslo
            };

            return rdpModel;
        }
    }
    [Table("PolaczeniaWebBrowser")]
    public class WebBrowserModel
    {
        public int Id { get; set; }
        public string AdresHttp { get; set; }
        public virtual ElementModel Element { get; set; }

        public static implicit operator WebBrowserModel(WebBrowser web)
        {
            ElementModel elementModel = new ElementModel
            {
                Klucz = web.Klucz,
                Nazwa = web.Nazwa,
                Opis = web.Opis,
                Rodzaj = web.Rodzaj,
                KluczRodzica = web.KluczRodzica
            };

            WebBrowserModel webBrowserModel = new WebBrowserModel()
            {
                Element = elementModel,
                AdresHttp = web.WebBrowserAddress
            };

            return webBrowserModel;
        }
    }

}
