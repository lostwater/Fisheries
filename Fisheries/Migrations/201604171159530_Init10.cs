namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ads", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ads", "Name");
        }
    }
}
