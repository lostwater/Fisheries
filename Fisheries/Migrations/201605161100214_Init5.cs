namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserLives", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserLives", "Live_Id", "dbo.Lives");
            DropIndex("dbo.ApplicationUserLives", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserLives", new[] { "Live_Id" });
            CreateTable(
                "dbo.UserLiveRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        CitizenId = c.String(),
                        LiveName = c.String(),
                        State = c.Int(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            AddColumn("dbo.Lives", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "LiveId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "Live_Id", c => c.Int());
            CreateIndex("dbo.Lives", "ApplicationUser_Id");
            CreateIndex("dbo.AspNetUsers", "LiveId");
            CreateIndex("dbo.AspNetUsers", "Live_Id");
            AddForeignKey("dbo.Lives", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "LiveId", "dbo.Lives", "Id");
            AddForeignKey("dbo.AspNetUsers", "Live_Id", "dbo.Lives", "Id");
            DropTable("dbo.ApplicationUserLives");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationUserLives",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Live_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Live_Id });
            
            DropForeignKey("dbo.UserLiveRequests", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Live_Id", "dbo.Lives");
            DropForeignKey("dbo.AspNetUsers", "LiveId", "dbo.Lives");
            DropForeignKey("dbo.Lives", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserLiveRequests", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Live_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "LiveId" });
            DropIndex("dbo.Lives", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.AspNetUsers", "Live_Id");
            DropColumn("dbo.AspNetUsers", "LiveId");
            DropColumn("dbo.Lives", "ApplicationUser_Id");
            DropTable("dbo.UserLiveRequests");
            CreateIndex("dbo.ApplicationUserLives", "Live_Id");
            CreateIndex("dbo.ApplicationUserLives", "ApplicationUser_Id");
            AddForeignKey("dbo.ApplicationUserLives", "Live_Id", "dbo.Lives", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserLives", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
