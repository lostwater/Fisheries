using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Fisheries.Models
{
    public class LiveType
    {
        public int Id { get; set; }
        [Display(Name ="类型名称")]
        public string Name { get; set; }
    }
}