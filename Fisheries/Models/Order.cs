using System;
using System.ComponentModel.DataAnnotations;

namespace Fisheries.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "下单时间")]
        public DateTime OrderTime { get; set; }

        [Display(Name = "订单价格")]
        [DataType(DataType.Currency)]
        public decimal OrderPrice { get; set; }

        [Display(Name = "订单描述")]
        public string Description { get; set; }

        [Display(Name = "商品数量")]
        public int Quantity { get; set; }

        
        public int OrderStatuId { get; set; }
        public OrderStatu OrderStatu { get; set; }

        public string Code { get; set; }
        public string PhoneNumber { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public int? PaymentId { get; set; }
        public virtual Payment Payment { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}