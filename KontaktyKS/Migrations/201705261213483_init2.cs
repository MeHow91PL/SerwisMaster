namespace SerwisMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.RdpModels", newName: "PolaczeniaRdp");
            RenameTable(name: "dbo.TeamViewerModels", newName: "PolaczeniaTeamViewer");
            RenameTable(name: "dbo.WebBrowserModels", newName: "PolaczeniaWebBrowser");
            RenameColumn(table: "dbo.Klienci", name: "Element_id_Id", newName: "Element_Id");
            RenameIndex(table: "dbo.Klienci", name: "IX_Element_id_Id", newName: "IX_Element_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Klienci", name: "IX_Element_Id", newName: "IX_Element_id_Id");
            RenameColumn(table: "dbo.Klienci", name: "Element_Id", newName: "Element_id_Id");
            RenameTable(name: "dbo.PolaczeniaWebBrowser", newName: "WebBrowserModels");
            RenameTable(name: "dbo.PolaczeniaTeamViewer", newName: "TeamViewerModels");
            RenameTable(name: "dbo.PolaczeniaRdp", newName: "RdpModels");
        }
    }
}
