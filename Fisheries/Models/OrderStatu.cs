using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class OrderStatu
    {
        public int Id { get; set; }
        [Display(Name = "状态")]
        public string Name { get; set; }

        // 1 = 待付款，2=已付款，3=取消，4=已使用}
    }
}