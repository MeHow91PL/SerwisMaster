namespace SerwisMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Elementy",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Klucz = c.String(nullable: false),
                        Rodzaj = c.Int(nullable: false),
                        Nazwa = c.String(),
                        Opis = c.String(),
                        Status = c.Int(nullable: false),
                        ParentId = c.Int(nullable: false),
                        KluczRodzica = c.String(),
                        ElementModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Elementy", t => t.ElementModel_Id)
                .Index(t => t.ElementModel_Id);
            
            CreateTable(
                "dbo.Klienci",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Element_id_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Elementy", t => t.Element_id_Id, cascadeDelete: true)
                .Index(t => t.Element_id_Id);
            
            CreateTable(
                "dbo.AdresyEmail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AdresEmail = c.String(),
                        Klient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Klienci", t => t.Klient_Id)
                .Index(t => t.Klient_Id);
            
            CreateTable(
                "dbo.DaneLogowania",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Haslo = c.String(),
                        System = c.Int(nullable: false),
                        Klient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Klienci", t => t.Klient_Id)
                .Index(t => t.Klient_Id);
            
            CreateTable(
                "dbo.Telefony",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nazwa = c.String(),
                        NumerTelefonu = c.String(),
                        Klient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Klienci", t => t.Klient_Id)
                .Index(t => t.Klient_Id);
            
            CreateTable(
                "dbo.Opcje",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nazwa = c.String(),
                        Wartosc = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RdpModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AdresRdp = c.String(),
                        Login = c.String(),
                        Haslo = c.String(),
                        Element_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Elementy", t => t.Element_Id)
                .Index(t => t.Element_Id);
            
            CreateTable(
                "dbo.TeamViewerModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamViewerId = c.String(),
                        Haslo = c.String(),
                        Element_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Elementy", t => t.Element_Id)
                .Index(t => t.Element_Id);
            
            CreateTable(
                "dbo.WebBrowserModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AdresHttp = c.String(),
                        Element_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Elementy", t => t.Element_Id)
                .Index(t => t.Element_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebBrowserModels", "Element_Id", "dbo.Elementy");
            DropForeignKey("dbo.TeamViewerModels", "Element_Id", "dbo.Elementy");
            DropForeignKey("dbo.RdpModels", "Element_Id", "dbo.Elementy");
            DropForeignKey("dbo.Telefony", "Klient_Id", "dbo.Klienci");
            DropForeignKey("dbo.Klienci", "Element_id_Id", "dbo.Elementy");
            DropForeignKey("dbo.DaneLogowania", "Klient_Id", "dbo.Klienci");
            DropForeignKey("dbo.AdresyEmail", "Klient_Id", "dbo.Klienci");
            DropForeignKey("dbo.Elementy", "ElementModel_Id", "dbo.Elementy");
            DropIndex("dbo.WebBrowserModels", new[] { "Element_Id" });
            DropIndex("dbo.TeamViewerModels", new[] { "Element_Id" });
            DropIndex("dbo.RdpModels", new[] { "Element_Id" });
            DropIndex("dbo.Telefony", new[] { "Klient_Id" });
            DropIndex("dbo.DaneLogowania", new[] { "Klient_Id" });
            DropIndex("dbo.AdresyEmail", new[] { "Klient_Id" });
            DropIndex("dbo.Klienci", new[] { "Element_id_Id" });
            DropIndex("dbo.Elementy", new[] { "ElementModel_Id" });
            DropTable("dbo.WebBrowserModels");
            DropTable("dbo.TeamViewerModels");
            DropTable("dbo.RdpModels");
            DropTable("dbo.Opcje");
            DropTable("dbo.Telefony");
            DropTable("dbo.DaneLogowania");
            DropTable("dbo.AdresyEmail");
            DropTable("dbo.Klienci");
            DropTable("dbo.Elementy");
        }
    }
}
