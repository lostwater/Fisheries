using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fisheries.Models
{
    public class Video
    {
        public int Id { set; get; }

        [Required]
        [Index("LCIndex", IsUnique = true)]
        [Display(Name = "乐视视频ID")]
        public int LCId { set; get; }

        [Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "截图")]
        public string ImageUrl { get; set; }

        
        public string VU { get; set; }

        public int? CelebrityId { get; set; }
        public virtual Celebrity Celebrity { get; set; }
    }
}