namespace Fisheries.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "CreateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Payments", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Payments", "RefundAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Payments", "Channel", c => c.String());
            AddColumn("dbo.Payments", "isPaid", c => c.Boolean(nullable: false));
            AddColumn("dbo.Payments", "isRefund", c => c.Boolean(nullable: false));
            AddColumn("dbo.Payments", "PingChargeId", c => c.String());
            AddColumn("dbo.Payments", "ChannelPaymentId", c => c.String());
            AddColumn("dbo.Payments", "ChannelPaymentUserId", c => c.String());
            DropColumn("dbo.Payments", "PaymentPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "PaymentPrice", c => c.Single(nullable: false));
            DropColumn("dbo.Payments", "ChannelPaymentUserId");
            DropColumn("dbo.Payments", "ChannelPaymentId");
            DropColumn("dbo.Payments", "PingChargeId");
            DropColumn("dbo.Payments", "isRefund");
            DropColumn("dbo.Payments", "isPaid");
            DropColumn("dbo.Payments", "Channel");
            DropColumn("dbo.Payments", "RefundAmount");
            DropColumn("dbo.Payments", "Amount");
            DropColumn("dbo.Payments", "CreateTime");
        }
    }
}
