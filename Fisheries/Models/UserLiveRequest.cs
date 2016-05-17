using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class UserLiveRequest
    {
        public int Id { get; set; }
        [Display(Name = "姓名")]
        public string FullName { get; set; }
        [Display(Name = "身份证")]
        public string CitizenId { get; set; }
        [Display(Name = "直播名")]
        public string LiveName { get; set; }

        //0:待分配，1:已分配，2:未通过
        public int State { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}