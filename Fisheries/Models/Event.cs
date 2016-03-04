using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace Fisheries.Models
{
    public class Event
    {
        public int Id { get; set; }

        public bool IsPublished { get; set; }

        [Display(Name = "放钓名称")]
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "活动开始时间")]
        public DateTime EventFrom { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "活动结束时间")]
        public DateTime EvenUntil { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "报名开始")]
        public DateTime RegeristFrom { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "报名结束")]
        public DateTime RegeristUntil { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [Display(Name = "折扣")]
        public decimal Discount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "折扣价")]
        public decimal DiscountPrice { get; set; }

        public DateTime? StartTime { get; set; }

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
}