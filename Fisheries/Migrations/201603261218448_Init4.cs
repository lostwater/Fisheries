namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Information", "InformationTypeId", "dbo.InformationTypes");
            DropIndex("dbo.Information", new[] { "InformationTypeId" });
            AlterColumn("dbo.Information", "InformationTypeId", c => c.Int());
            CreateIndex("dbo.Information", "InformationTypeId");
            AddForeignKey("dbo.Information", "InformationTypeId", "dbo.InformationTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Information", "InformationTypeId", "dbo.InformationTypes");
            DropIndex("dbo.Information", new[] { "InformationTypeId" });
            AlterColumn("dbo.Information", "InformationTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Information", "InformationTypeId");
            AddForeignKey("dbo.Information", "InformationTypeId", "dbo.InformationTypes", "Id", cascadeDelete: true);
        }
    }
}
