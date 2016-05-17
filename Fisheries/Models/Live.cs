using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;
using System.Runtime.Serialization;

namespace Fisheries.Models
{
    [DataContract]
    public class Live
    {
        public Live()
        {
            this.FollowedUsers = new HashSet<ApplicationUser>();
        }

        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        [Display(Name = "乐视Id")]
        [Required]
        [Index("IX_CloudLiveId", 2, IsUnique = true)]
        public string CloudLiveId { get; set; }
        [DataMember]
        [ForeignKey("CloudLiveId")]
        public virtual CloudLive CloudLive { get; set; }
        [DataMember]
        [Display(Name = "类型")]
        public int? LiveTypeId { get; set; }
        [DataMember]
        public virtual LiveType LiveType { get; set; }
        [DataMember]
        [Display(Name = "聊天室ID")]
        public string ChatId { get; set; }
   


        public virtual ICollection<ApplicationUser> FollowedUsers { get; set; }
        //[Display(Name = "渔场")]
        // [ForeignKey("Shop")]
        // public int? ShopId { get; set; }
        //public virtual Shop Shop { get; set; }

        //[Display(Name = "赛事")]
        //public int? EventId { get; set; }
        // public virtual Event Event { get; set; }
    }

    public class LiveBindModel
    { 

        public int Id { get; set; }

        [Display(Name = "乐视Id")]
        public string CloudLiveId { get; set; }
        public CloudLive CloudLive { get; set; }

        [Display(Name = "类型")]
        public int? LiveTypeId { get; set; }
        public LiveType LiveType { get; set; }


        [Display(Name = "渔场")]
        public int? ShopId { get; set; }
        public Shop Shop { get; set; }

        [Display(Name = "赛事")]
        public int? EventId{ get; set; }
        public Event Event { get; set; }

        [Display(Name = "主播")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public LiveBindModel()
        {

        }

        public LiveBindModel(Live live)
        {
            var db = new ApplicationDbContext();
            Id = live.Id;
            CloudLiveId = live.CloudLiveId;
            LiveTypeId = live.LiveTypeId;
            LiveType = live.LiveType;
            CloudLive = live.CloudLive;
            var _shop = db.Shops.FirstOrDefault(s => s.LiveId == Id);
            var _event = db.Events.FirstOrDefault(e => e.LiveId == Id);
            var _user = db.Users.FirstOrDefault(u => u.LiveId == Id);
            if (_shop != null)
            {
                ShopId = _shop.Id;
                Shop = _shop;
             }
            if (_event != null)
            {
                EventId = _event.Id;
                Event = _event;
            }
            if (_user != null)
            {
                ApplicationUserId = _user.Id;
                ApplicationUser = _user;
            }
        }
    }

}