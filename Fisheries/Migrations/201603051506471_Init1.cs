namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ads", "AvatarUrl", c => c.String());
            AddColumn("dbo.Events", "AvatarUrl", c => c.String());
            AddColumn("dbo.Shops", "AvatarUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "AvatarUrl");
            DropColumn("dbo.Events", "AvatarUrl");
            DropColumn("dbo.Ads", "AvatarUrl");
        }
    }
}
