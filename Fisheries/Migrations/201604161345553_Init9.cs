namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "Longitude", c => c.Double(nullable: false));
            AddColumn("dbo.Shops", "Latitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "Latitude");
            DropColumn("dbo.Shops", "Longitude");
        }
    }
}
