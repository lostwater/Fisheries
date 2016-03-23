using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Fisheries.Models
{
    public class Ad
    {
        public int Id { get; set; }
        [Display(Name = "广告图")]
        public string AvatarUrl { get; set; }
        public int AdCat { get; set; }
        public int AdType { get; set; }
        public int? EventId { get; set; }
        public virtual Event Event { get; set; }
        public int? InformationId { get; set; }
        public virtual Information Information { get; set; }
    }

    public class AdEditModel
    {
        public int Id { get; set; }
        [Display(Name = "广告图")]
        public string AvatarUrl { get; set; }
        [Display(Name = "广告位于")]
        public int AdCat { get; set; }

        public int AdType { get; set; }

        public int? EventId { get; set; }
        public virtual Event Event { get; set; }
        public int? InformationId { get; set; }
        public virtual Information Information { get; set; }
    }
}