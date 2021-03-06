﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Fisheries.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("fisheries_dbEntities", throwIfV1Schema: false)
        {
            this.Configuration.LazyLoadingEnabled = true;
            //this.Database.Connection.ConnectionString = this.Database.Connection.ConnectionString + ";Password = jimmysill";
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Shop>().
             HasMany(l =>l.FollowedUsers).
             WithMany(u => u.FollowedShops).
            Map(
            m =>
            {
           m.MapLeftKey("ApplicationUserId");
           m.MapRightKey("ShopId");
           m.ToTable("ApplicationUserShops");
            });
            
        }

        public System.Data.Entity.DbSet<Fisheries.Models.Shop> Shops { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Event> Events { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Information> Information { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Order> Orders { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Payment> Payments { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.OrderStatu> OrderStatus { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.InformationType> InformationTypes { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Ad> Ads { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Celebrity> Celebrities { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Video> Videos { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.Live> Lives { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.CloudLive> CloudLives { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.LiveType> LiveTypes { get; set; }

        public System.Data.Entity.DbSet<Fisheries.Models.UserLiveRequest> UserLiveRequests { get; set; }


        //public System.Data.Entity.DbSet<Client> Clients { get; set; }

        // public System.Data.Entity.DbSet<Fisheries.Models.ApplicationUser> ApplicationUsers { get; set; }
        //public System.Data.Entity.DbSet<Fisheries.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}