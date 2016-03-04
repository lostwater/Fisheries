using System;
using System.ComponentModel.DataAnnotations;


namespace Fisheries.Models
{
    public class OrderCodeVerifyModel
    {
        [Display(Name = "手机号")]
        public string PhoneNumber { get; set; }
        [Display(Name = "验证码")]
        public string Code { get; set; }
    }
}