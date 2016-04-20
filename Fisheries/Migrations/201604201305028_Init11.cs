namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CreatedTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "SignupClient", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SignupClient");
            DropColumn("dbo.AspNetUsers", "CreatedTime");
        }
    }
}
