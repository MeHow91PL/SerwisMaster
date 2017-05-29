using SerwisMaster.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SerwisMaster.DAL
{
    class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
          
        }
        public DbSet<ElementModel> Elementy { get; set; }
        public DbSet<KlientModel> Klienci { get; set; }
        public DbSet<TeamViewerModel> PolaczeniaTeamViewer { get; set; }
        public DbSet<RdpModel> PolaczeniaRdp { get; set; }
        public DbSet<WebBrowserModel> PolaczeniaWebBrowser { get; set; }
        public DbSet<Opcje> Opcje { get; set; }
    }
}
