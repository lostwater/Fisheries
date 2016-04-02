using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.Owin;
using Owin;
using Hangfire;
using Fisheries.Models;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

[assembly: OwinStartup(typeof(Fisheries.Startup))]

namespace Fisheries
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("fisheries_dbEntities");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            ConfigureAuth(app);

            RecurringJob.AddOrUpdate(() => updateOrderStatu(), Cron.Minutely);
        }

        public void updateOrderStatu()
        {
            var db = new ApplicationDbContext();
            var orders = db.Orders.Where(o => o.OrderStatuId == 1).ToList();
            orders = orders.Where(o => (DateTime.Now - o.OrderTime) > new TimeSpan(0, 30, 0)).ToList();
            //var aorders = orders.Where(o => (DateTime.Now - o.OrderTime).TotalSeconds > 30 * 60).ToList();
            //var orders = db.Orders.Where(o => o.OrderStatuId == 1).Where(o => (o.OrderTime + new TimeSpan(0, 30, 0)).Ticks < DateTime.Now.Ticks).ToList();
            foreach (var o in orders)
            {
                o.OrderStatuId = 4;
            }
            db.SaveChanges();

        }
    }
}
