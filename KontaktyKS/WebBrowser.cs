using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerwisMaster
{
    class WebBrowser : Element
    {
        public string WebBrowserApplication { get; set; }
        public string WebBrowserAddress { get; set; }

        public WebBrowser(string name, string group, string description, string webBrowserApplication, 
            string webBrowserAddress, string id="", object parent=null) 
            : base(name, group,description,id,parent)
        {
            this.Nazwa = name;
            this.KluczRodzica = group;
            this.Rodzaj = Models.RodzajElementu.WebBrowser;
            this.WebBrowserApplication = webBrowserApplication;
            this.WebBrowserAddress = webBrowserAddress;
        }
    }
}
