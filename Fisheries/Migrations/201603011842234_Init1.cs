namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Events", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Events", "DiscountPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Events", "BuyPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Orders", "OrderPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "OrderPrice", c => c.Single(nullable: false));
            AlterColumn("dbo.Events", "BuyPrice", c => c.Single(nullable: false));
            AlterColumn("dbo.Events", "DiscountPrice", c => c.Single(nullable: false));
            AlterColumn("dbo.Events", "Discount", c => c.Single(nullable: false));
            AlterColumn("dbo.Events", "Price", c => c.Single(nullable: false));
        }
    }
}
