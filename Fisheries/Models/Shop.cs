using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Fisheries.Models
{
    [DataContract]
    public class Shop
    {
        public Shop()
        {
            this.FollowedUsers = new HashSet<ApplicationUser>();
            Events = new HashSet<Event>();
        }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        [Display(Name = "名称")]
        public string Name { get; set; }
        [DataMember]
        [Display(Name = "渔场图地址")]
        public string AvatarUrl { get; set; }
        [DataMember]
        [Display(Name = "地址")]
        public string Address { get; set; }
        [DataMember]
        [Display(Name = "介绍")]
        public string Intro { get; set; }
        [DataMember]
        [Display(Name = "描述")]
        public string Description { get; set; }
        [DataMember]
        [Display(Name = "面积")]
        public decimal Surface { get; set; }
        [DataMember]
        [Display(Name = "是否已认证")]
        public bool Verified { get; set; }
        [DataMember]
        [Display(Name = "经度")]
        public double Longitude { get; set; }
        [DataMember]
        [Display(Name = "纬度")]
        public double Latitude { get; set; }
        [DataMember]
        [Display(Name = "商家用户ID")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [DataMember]
        public int? LiveId { get; set; }
        public virtual Live Live { get; set; }

        
        public virtual ICollection<ApplicationUser> FollowedUsers { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}