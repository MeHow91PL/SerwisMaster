namespace SerwisMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Elementy",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Klucz = c.String(),
                        Rodzaj = c.Int(nullable: false),
                        Nazwa = c.String(),
                        Opis = c.String(),
                        ParentId = c.Int(nullable: false),
                        KluczRodzica = c.String(),
                        ElementModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Elementy", t => t.ElementModel_Id)
                .Index(t => t.ElementModel_Id);
            
            CreateTable(
                "dbo.Kliencis",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        haslo = c.String(),
                        Element_Id = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Elementy", t => t.Element_Id)
                .Index(t => t.Element_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Kliencis", "Element_Id", "dbo.Elementy");
            DropForeignKey("dbo.Elementy", "ElementModel_Id", "dbo.Elementy");
            DropIndex("dbo.Kliencis", new[] { "Element_Id" });
            DropIndex("dbo.Elementy", new[] { "ElementModel_Id" });
            DropTable("dbo.Kliencis");
            DropTable("dbo.Elementy");
        }
    }
}
