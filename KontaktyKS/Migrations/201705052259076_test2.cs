namespace SerwisMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Elementy",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rodzaj = c.Int(nullable: false),
                        Nazwa = c.String(),
                        IdRodzica = c.Int(nullable: false),
                        Elementy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Elementy", t => t.Elementy_Id)
                .Index(t => t.Elementy_Id);
            
            AddColumn("dbo.Kliencis", "haslo", c => c.String());
            AddColumn("dbo.Kliencis", "Element_Id", c => c.Int());
            CreateIndex("dbo.Kliencis", "Element_Id");
            AddForeignKey("dbo.Kliencis", "Element_Id", "dbo.Elementy", "Id");
            DropColumn("dbo.Kliencis", "Nazwa");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Kliencis", "Nazwa", c => c.String());
            DropForeignKey("dbo.Kliencis", "Element_Id", "dbo.Elementy");
            DropForeignKey("dbo.Elementy", "Elementy_Id", "dbo.Elementy");
            DropIndex("dbo.Kliencis", new[] { "Element_Id" });
            DropIndex("dbo.Elementy", new[] { "Elementy_Id" });
            DropColumn("dbo.Kliencis", "Element_Id");
            DropColumn("dbo.Kliencis", "haslo");
            DropTable("dbo.Elementy");
        }
    }
}
