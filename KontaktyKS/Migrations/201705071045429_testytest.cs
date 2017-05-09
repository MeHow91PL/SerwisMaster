namespace SerwisMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testytest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Elementy", "Opis", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Elementy", "Opis");
        }
    }
}
