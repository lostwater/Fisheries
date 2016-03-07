namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Celebrities", "Intro", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Celebrities", "Intro");
        }
    }
}
