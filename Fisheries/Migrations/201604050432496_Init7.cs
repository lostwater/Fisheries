namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "EventFrom", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Events", "EvenUntil", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "EvenUntil", c => c.DateTime());
            AlterColumn("dbo.Events", "EventFrom", c => c.DateTime());
        }
    }
}
