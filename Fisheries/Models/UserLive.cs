using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class UserLive
    {
        public int Id { get; set; }
        public String ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}