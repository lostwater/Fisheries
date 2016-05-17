using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class FollowLives
    {
        [Key]
        public String ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Key]
        public String LiveId { get; set; }
        public virtual Live Live { get; set; }
    }
}