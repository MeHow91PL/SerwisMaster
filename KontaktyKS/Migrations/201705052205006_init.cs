namespace SerwisMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Kliencis",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nazwa = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Kliencis");
        }
    }
}
