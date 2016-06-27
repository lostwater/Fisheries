using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fisheries.Models;
using Newtonsoft.Json;
using RestSharp;
using Fisheries.Helper;

namespace Fisheries.Controllers
{
    public class LivesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Lives
        public async Task<ActionResult> Index()
        {
            var lives = db.Lives.Include(l => l.CloudLive).Where(l => l.CloudLive.activityStatus != 3);
            var models = new List<LiveBindModel>();
            await lives.ForEachAsync(l => models.Add(new LiveBindModel(l)));
            return View(models);
        }

        // GET: Lives/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Live live = await db.Lives.FindAsync(id);
            if (live == null)
            {
                return HttpNotFound();
            }
            return View(live);
        }


        public ActionResult SyncCloud()
        {
            new LiveSync().Sync();

            return RedirectToAction("Index");
        }

      
         public ActionResult Create()
        {
            ViewBag.CloudLiveId = new SelectList(db.CloudLives, "activityId", "activityName");
            //ViewBag.EventId = new SelectList(db.Events, "Id", "Name");
            //ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name");
            return View();
        }

        // POST: Lives/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CloudLiveId,ShopId,EventId")] Live live)
        {
            if (ModelState.IsValid)
            {
                db.Lives.Add(live);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LiveTypeId = new SelectList(db.LiveTypes, "Id", "Name", live.LiveTypeId); 
            ViewBag.CloudLiveId = new SelectList(db.CloudLives, "activityId", "activityName", live.CloudLiveId);
            //ViewBag.EventId = new SelectList(db.Events, "Id", "Name", live.EventId);
            //ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", live.ShopId);
            return View(live);
        }

        // GET: Lives/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Live live = await db.Lives.FindAsync(id);
            if (live == null)
            {
                return HttpNotFound();
            }
            var model = new LiveBindModel(live);
            ViewBag.LiveTypeId = new SelectList(db.LiveTypes, "Id", "Name", model.LiveTypeId);
            ViewBag.CloudLiveId = new SelectList(db.CloudLives, "activityId", "activityName", model.CloudLiveId);
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", model.EventId);
            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", model.ShopId);
            var users = db.UserLiveRequests.Where(r => r.State == 0).Select(r => r.ApplicationUser);
            ViewBag.ApplicationUserId = new SelectList(users, "Id", "UserName", model.ApplicationUserId);
            return View(model);
        }

        // POST: Lives/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LiveBindModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ShopId.HasValue)
                {
                    var shop = db.Shops.Find(model.ShopId.Value);
                    shop.LiveId = model.Id;
                }
                if (model.EventId.HasValue)
                {
                    var @event = db.Events.Find(model.EventId.Value);
                    @event.LiveId = model.Id;
                }
                var user = db.Users.Find(model.ApplicationUserId);
                if (user != null)
                    user.LiveId = model.Id;
                var live = db.Lives.Find(model.Id);
                live.LiveTypeId = model.LiveTypeId;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LiveTypeId = new SelectList(db.LiveTypes, "Id", "Name", model.LiveTypeId);
            ViewBag.CloudLiveId = new SelectList(db.CloudLives, "activityId", "activityName", model.CloudLiveId);
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", model.EventId);
            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", model.ShopId);
            var users = db.UserLiveRequests.Where(r => r.State == 0).Select(r => r.ApplicationUser);
            ViewBag.ApplicationUserId = new SelectList(users, "Id", "UserName", model.ApplicationUserId);
            return View(model);
        }

        // GET: Lives/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Live live = await db.Lives.FindAsync(id);
            if (live == null)
            {
                return HttpNotFound();
            }
            return View(live);
        }

        // POST: Lives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Live live = await db.Lives.FindAsync(id);
            db.Lives.Remove(live);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
