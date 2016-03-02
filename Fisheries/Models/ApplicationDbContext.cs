using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Fisheries.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Fisheries.Models.Shop> Shops { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Event> Events { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Information> Information { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Order> Orders { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Payment> Payments { get; set; }

       // public System.Data.Entity.DbSet<Fisheries.Models.ApplicationUser> ApplicationUsers { get; set; }
        //public System.Data.Entity.DbSet<Fisheries.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}