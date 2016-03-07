namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Celebrities", "AvatarUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Celebrities", "AvatarUrl");
        }
    }
}
