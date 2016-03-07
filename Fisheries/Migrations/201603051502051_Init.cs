namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Celebrities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Information", "CelebrityId", c => c.Int());
            CreateIndex("dbo.Information", "CelebrityId");
            AddForeignKey("dbo.Information", "CelebrityId", "dbo.Celebrities", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Information", "CelebrityId", "dbo.Celebrities");
            DropIndex("dbo.Information", new[] { "CelebrityId" });
            DropColumn("dbo.Information", "CelebrityId");
            DropTable("dbo.Celebrities");
        }
    }
}
