namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Lives", "EventId", "dbo.Events");
            DropForeignKey("dbo.Lives", "ShopId", "dbo.Shops");
            DropIndex("dbo.Lives", new[] { "ShopId" });
            DropIndex("dbo.Lives", new[] { "EventId" });
            RenameColumn(table: "dbo.Shops", name: "Live_Id", newName: "LiveId");
            RenameIndex(table: "dbo.Shops", name: "IX_Live_Id", newName: "IX_LiveId");
            AddColumn("dbo.Events", "LiveId", c => c.Int());
            CreateIndex("dbo.Events", "LiveId");
            AddForeignKey("dbo.Events", "LiveId", "dbo.Lives", "Id");
            DropColumn("dbo.Lives", "ShopId");
            DropColumn("dbo.Lives", "EventId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lives", "EventId", c => c.Int());
            AddColumn("dbo.Lives", "ShopId", c => c.Int());
            DropForeignKey("dbo.Events", "LiveId", "dbo.Lives");
            DropIndex("dbo.Events", new[] { "LiveId" });
            DropColumn("dbo.Events", "LiveId");
            RenameIndex(table: "dbo.Shops", name: "IX_LiveId", newName: "IX_Live_Id");
            RenameColumn(table: "dbo.Shops", name: "LiveId", newName: "Live_Id");
            CreateIndex("dbo.Lives", "EventId");
            CreateIndex("dbo.Lives", "ShopId");
            AddForeignKey("dbo.Lives", "ShopId", "dbo.Shops", "Id");
            AddForeignKey("dbo.Lives", "EventId", "dbo.Events", "Id");
        }
    }
}
