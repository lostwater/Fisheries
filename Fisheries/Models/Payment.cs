using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime? PaymentTime { get; set; }
        public DateTime CreateTime { get; set; }
        public Decimal Amount { get; set; }
        public Decimal RefundAmount { get; set; }
        public string Description { get; set; }
        public string Channel { get; set; }
        public bool isPaid { get; set; }
        public bool isRefund { get; set; }
        public string PingChargeId { get; set; }
        public string ChannelPaymentId { get; set; }
        public string ChannelPaymentUserId { get; set; }

    }
}