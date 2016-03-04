﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Fisheries.Models
{
    public class Shop
    {
        public int Id { get; set; }
        [Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "地址")]
        public string Address { get; set; }
        [Display(Name = "介绍")]
        public string Intro { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "商家用户ID")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        
    }
}