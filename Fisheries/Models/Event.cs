using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Web;

namespace Fisheries.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Display(Name = "是否发布")]
        public bool IsPublished { get; set; }
        [Display(Name = "标题图")]
        public string AvatarUrl { get; set; }

        [Display(Name = "放钓名称")]
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "活动开始时间")]
        public DateTime? EventFrom { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "活动结束时间")]
        public DateTime? EvenUntil { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "报名开始")]
        public DateTime? RegeristFrom { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "报名结束")]
        public DateTime? RegeristUntil { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [Display(Name = "折扣")]
        public decimal Discount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "折扣价")]
        public decimal DiscountPrice { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "打氧时间")]
        public DateTime? OxygenTime { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "回收价")]
        public decimal BuyPrice { get; set; }

        [Display(Name = "鱼种")]
        public string FishType { get; set; }
        [Display(Name = "放钓量")]
        public float FishQuantity { get; set; }
        [Display(Name = "总钓位")]
        public int Positions { get; set; }
        [Display(Name = "剩余钓位")]
        public int PositionsRemain { get; set; }

        [Display(Name = "更多描述")]
        public string Description { get; set; }

        [Display(Name = "玩法介绍")]
        public string Intro { get; set; }

        public int ShopId { get; set; }
        public Shop Shop { get; set; }
    }

    public class EventEditModel
    {
        public int Id { get; set; }
        [Display(Name = "是否发布")]
        public bool IsPublished { get; set; }
        [Display(Name = "标题图")]
        public string AvatarUrl { get; set; }
        [Display(Name = "标题图")]
        public HttpPostedFileBase Avatar { get; set; }

        [Display(Name = "放钓名称")]
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "活动开始时间")]
        [Required]
        public DateTime? EventFrom { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "活动结束时间")]
        public DateTime? EvenUntil { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "报名开始")]
        public DateTime? RegeristFrom { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "报名结束")]
        public DateTime? RegeristUntil { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [Display(Name = "折扣")]
        public decimal Discount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "折扣价")]
        public decimal DiscountPrice { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "打氧时间")]
        public DateTime? OxygenTime { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "回收价")]
        public decimal BuyPrice { get; set; }

        [Display(Name = "鱼种")]
        public string FishType { get; set; }
        [Display(Name = "放钓量(斤)")]
        public float FishQuantity { get; set; }
        [Display(Name = "总钓位")]
        public int Positions { get; set; }
        [Display(Name = "剩余钓位")]
        public int PositionsRemain { get; set; }
        [Display(Name = "更多描述")]
        public string Description { get; set; }

        [Display(Name = "玩法介绍")]
        public string Intro { get; set; }

        public int ShopId { get; set; }


        public EventEditModel()
        { }

        public EventEditModel(Event e)
        {
            Id = e.Id;
            IsPublished = e.IsPublished;
            Name = e.Name;
            EventFrom = e.EventFrom;
            EvenUntil = e.EvenUntil;
            RegeristFrom = e.RegeristFrom;
            RegeristUntil = e.RegeristUntil;
            Price = e.Price;
            Discount = e.Discount;
            DiscountPrice = e.DiscountPrice;
            OxygenTime = e.OxygenTime;
            BuyPrice = e.BuyPrice;
            FishType = e.FishType;
            FishQuantity = e.FishQuantity;
            Positions = e.Positions;
            PositionsRemain = e.PositionsRemain;
            Description = e.Description;
            Intro = e.Intro;
            ShopId = e.ShopId;
            AvatarUrl = e.AvatarUrl;
        }
    }
}