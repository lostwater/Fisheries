namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUserLives",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Live_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Live_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Lives", t => t.Live_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Live_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserLives", "Live_Id", "dbo.Lives");
            DropForeignKey("dbo.ApplicationUserLives", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserLives", new[] { "Live_Id" });
            DropIndex("dbo.ApplicationUserLives", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationUserLives");
        }
    }
}
