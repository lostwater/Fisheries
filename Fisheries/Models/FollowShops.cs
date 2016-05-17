using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class FollowShops
    {
        [Key]
        public String ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Key]
        public int ShopId { get; set; }
        public virtual Shop Shop { get; set; }
    }
}