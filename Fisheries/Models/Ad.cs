using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class Ad
    {
        public int Id { get; set; }
        public string AvatarUrl { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}