using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Fisheries.Models
{
    public class Information
    {
        public int Id { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }
        [Display(Name = "视频地址")]
        public string VideoUrl { get; set; }
        [Display(Name = "主题图")]
        public string ImageUrl { get; set; }
        [Display(Name = "简介")]
        public string Intro { get; set; }

        [AllowHtml]
        [Display(Name = "内容")]
        public string Content { get; set; }
        [Display(Name = "创建时间")]
        public DateTime? CreatedTime { get; set; }
        [Display(Name = "发布时间")]
        public DateTime? PublishedTime { get; set; }

        [Display(Name = "是否发布")]
        public bool IsPublished { get; set; }

        [Display(Name = "信息类型")]
        public int? InformationTypeId { get; set; }
        public virtual InformationType InformationType { get; set; }

        [Display(Name = "名人")]
        public int? CelebrityId { get; set; }
        public virtual Celebrity Celebrity { get; set; }

        [Display(Name = "发布人")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    public class InformationEditModel
    {
        public int Id { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }
        [Display(Name = "视频地址")]
        public string VideoUrl { get; set; }
        [Display(Name = "视频地址")]
        public string ImageUrl { get; set; }
        [Display(Name = "简介")]
        public string Intro { get; set; }

        [AllowHtml]
        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreatedTime { get; set; }
        [Display(Name = "发布时间")]
        public DateTime? PublishedTime { get; set; }

        [Display(Name = "是否发布")]
        public bool IsPublished { get; set; }
        [Display(Name = "信息类型")]
        public int? InformationTypeId { get; set; }
        public virtual InformationType InformationType { get; set; }
        
        [Display(Name = "主图片")]
        public HttpPostedFileBase Image { get; set; }

        [Display(Name = "名人")]
        public int? CelebrityId { get; set; }
        public virtual Celebrity Celebrity { get; set; }

        public InformationEditModel()
        { }

        public InformationEditModel(Information information)
        {
            this.InformationTypeId = information.InformationTypeId;
            this.Content = information.Content;
            this.Id = information.Id;
            this.ImageUrl = information.ImageUrl;
            this.VideoUrl = information.VideoUrl;
            this.CreatedTime = information.CreatedTime;
            this.PublishedTime = information.PublishedTime;
            this.Title = information.Title;
            this.Intro = information.Intro;
            this.IsPublished = information.IsPublished;
            this.CelebrityId = information.CelebrityId;
        }
     }

}