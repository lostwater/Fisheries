using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Fisheries.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据，若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        
        public ApplicationUser()
        {
            FollowedShops = new HashSet<Shop>();
            FollowedLives = new HashSet<Live>();
    
        }
        public virtual ICollection<Shop> FollowedShops { get; set; }
        public virtual ICollection<Live> FollowedLives { get; set; }

        public int? LiveId { get; set; }
        public virtual Live Live { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedTime { get; set; }
        public string SignupClient { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // 在此处添加自定义用户声明
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
           
        }
    }

   

    

}