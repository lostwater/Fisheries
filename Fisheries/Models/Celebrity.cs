using System.ComponentModel.DataAnnotations;

namespace Fisheries.Models
{
    public class Celebrity
    {
        public int Id { get; set; }
        [Display(Name = "姓名")]   
        public string Name { get; set; }
        [Display(Name = "简介")]
        public string Intro { get; set; }
        [Display(Name = "头像")]
        public string AvatarUrl { get; set; }
    }
}