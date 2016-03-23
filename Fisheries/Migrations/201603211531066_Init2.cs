namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Information", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Information", "ApplicationUserId");
            AddForeignKey("dbo.Information", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Information", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Information", new[] { "ApplicationUserId" });
            DropColumn("dbo.Information", "ApplicationUserId");
        }
    }
}
