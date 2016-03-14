namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Ads", "EventId", "dbo.Events");
            DropIndex("dbo.Ads", new[] { "EventId" });
            AddColumn("dbo.Ads", "InformationId", c => c.Int());
            AddColumn("dbo.Shops", "Verified", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Ads", "EventId", c => c.Int());
            CreateIndex("dbo.Ads", "EventId");
            CreateIndex("dbo.Ads", "InformationId");
            AddForeignKey("dbo.Ads", "InformationId", "dbo.Information", "Id");
            AddForeignKey("dbo.Ads", "EventId", "dbo.Events", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ads", "EventId", "dbo.Events");
            DropForeignKey("dbo.Ads", "InformationId", "dbo.Information");
            DropIndex("dbo.Ads", new[] { "InformationId" });
            DropIndex("dbo.Ads", new[] { "EventId" });
            AlterColumn("dbo.Ads", "EventId", c => c.Int(nullable: false));
            DropColumn("dbo.Shops", "Verified");
            DropColumn("dbo.Ads", "InformationId");
            CreateIndex("dbo.Ads", "EventId");
            AddForeignKey("dbo.Ads", "EventId", "dbo.Events", "Id", cascadeDelete: true);
        }
    }
}
