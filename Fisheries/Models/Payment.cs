using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Fisheries.Models
{
    public class Payment
    {
        public int Id { get; set; }
        [Display(Name = "支付时间")]
        public DateTime? PaymentTime { get; set; }
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }
        [Display(Name = "支付总额")]
        public Decimal Amount { get; set; }
        [Display(Name = "退款总额")]
        public Decimal RefundAmount { get; set; }
        public string Description { get; set; }
        [Display(Name = "支付渠道")]
        public string Channel { get; set; }
        [Display(Name = "是否支付")]
        public bool isPaid { get; set; }
        [Display(Name = "是否退款")]
        public bool isRefund { get; set; }
        public string PingChargeId { get; set; }
        public string ChannelPaymentId { get; set; }
        public string ChannelPaymentUserId { get; set; }

    }
}