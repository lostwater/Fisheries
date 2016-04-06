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
            var events = db.Events.Where(e => e.IsPublished).ToList();
            foreach(var e in events)
            {
                e.PositionsRemain = e.Positions - db.Orders.Count(o => o.EventId == e.Id && o.OrderStatuId != 4);
            }
            db.SaveChanges();
            events = events.Where(e => e.EventFrom.DayOfYear <= DateTime.Now.DayOfYear && e.EventFrom.Year <= DateTime.Now.Year).ToList();
            foreach (var e in events)
            {
                e.IsPublished = false;
            }
            db.SaveChanges();


        }
    }
}
