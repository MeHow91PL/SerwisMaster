using SerwisMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerwisMaster
{
    public class WebBrowser : Element
    {
        public string WebBrowserApplication { get; set; }
        public string WebBrowserAddress { get; set; }

        public WebBrowser(string name, string kluczRodzica, string description, 
            string webBrowserAddress, string klucz="", object parent=null) 
            : base(name, kluczRodzica,description,klucz,parent)
        {
            this.Nazwa = name;
            this.KluczRodzica = kluczRodzica;
            this.Rodzaj = Models.RodzajElementu.WebBrowser;
            this.WebBrowserAddress = webBrowserAddress;
        }

        public static implicit operator WebBrowser(WebBrowserModel webBrowserModel)
        {
            WebBrowser webBrowser = new WebBrowser(
                webBrowserModel.Element.Nazwa,
                webBrowserModel.Element.KluczRodzica,
                webBrowserModel.Element.Opis,
                webBrowserModel.AdresHttp,
                webBrowserModel.Element.Klucz
            );
            return webBrowser;
        }
    }
}
