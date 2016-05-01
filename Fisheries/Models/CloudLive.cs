using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Fisheries.Models
{
    public class CloudLive
    {
        [Key]
        [Display(Name = "活动ID")]
        public string activityId { get; set; }
        [Display(Name = "活动名称")]
        public string activityName { get; set; }
        [Display(Name = "活动状态")]
        public int activityStatus { get; set; }
        [Display(Name = "封面")]
        public string coverImgUrl { get; set; }
        [Display(Name = "创建时间")]
        public string createTime { get; set; }
        [Display(Name = "描述")]
        public string description { get; set; }
        [Display(Name = "结束时间")]
        public string endTime { get; set; }
        public int liveNum { get; set; }
        public int needIpWhiteList { get; set; }
        public int needRecord { get; set; }
        public int needFullView { get; set; }
        public int needTimeShift { get; set; }
        public int neededPushAuth { get; set; }
        public string pushIpWhiteList { get; set; }
        public int pushUrlValidTime { get; set; }
        [Display(Name = "开始时间")]
        public string startTime { get; set; }
        public int userCount { get; set; }
        public int playMode { get; set; }
    }
}