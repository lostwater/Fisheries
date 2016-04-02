using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class OrderStatu
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // 0 = 待付款，1=已付款，2=取消，3=已使用}
    }
}