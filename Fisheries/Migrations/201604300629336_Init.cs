namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "Live_Id", c => c.Int());
            CreateIndex("dbo.Shops", "Live_Id");
            AddForeignKey("dbo.Shops", "Live_Id", "dbo.Lives", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shops", "Live_Id", "dbo.Lives");
            DropIndex("dbo.Shops", new[] { "Live_Id" });
            DropColumn("dbo.Shops", "Live_Id");
        }
    }
}
