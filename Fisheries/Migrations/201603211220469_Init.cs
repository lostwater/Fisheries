namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ads", "AdType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ads", "AdType");
        }
    }
}
