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
        public int? EventId { get; set; }
        public virtual Event Event { get; set; }
        public int? InformationId { get; set; }
        public virtual Information Information { get; set; }
    }
}