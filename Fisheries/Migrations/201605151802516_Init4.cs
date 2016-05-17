namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUserShops",
                c => new
                    {
                        ApplicationUserId = c.Int(nullable: false),
                        ShopId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.ShopId })
                .ForeignKey("dbo.Shops", t => t.ApplicationUserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ShopId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.ShopId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserShops", "ShopId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserShops", "ApplicationUserId", "dbo.Shops");
            DropIndex("dbo.ApplicationUserShops", new[] { "ShopId" });
            DropIndex("dbo.ApplicationUserShops", new[] { "ApplicationUserId" });
            DropTable("dbo.ApplicationUserShops");
        }
    }
}
