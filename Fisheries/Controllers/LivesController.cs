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


        public async Task<ActionResult> SyncCloud()
        {
            var lcsdk = new Gandalf.LCUploader("nal4hqaahb", "2e44b05a1d3b751efc6a3a3eb1654e79");
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("ver", "3.0");
            args.Add("userid", 823100);
            args.Add("method", "letv.cloudlive.activity.search");

            string retUrl = lcsdk.handleLiveParam("http://api.open.letvcloud.com/live/execute", args);
            string strResult = lcsdk.doRequest(retUrl);
            //return strResult.Replace("\\", "");
            List<CloudLive> list = JsonConvert.DeserializeObject<List<CloudLive>>(strResult);
            list = list.Where(l => l.activityStatus != 3).ToList();
            list.ForEach(cl =>
                 {
                     if (!db.CloudLives.Any(_cl => _cl.activityId == cl.activityId))
                     {
                         db.CloudLives.Add(cl);
                     }
                     else
                     {
                         //db.CloudLives.Attach(cl);
                         db.Entry(cl).State = EntityState.Modified;
                     }
            
                     if(!db.Lives.Any(l=>l.CloudLiveId == cl.activityId))
                     {
                         var live = new Live()
                         {
                             CloudLiveId = cl.activityId,
                             CloudLive = cl
                         };
                         db.Lives.Add(live);
                     }
                 }
            );
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: Lives/Create
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
                var live = db.Lives.Find(model.Id);
                live.LiveTypeId = model.LiveTypeId;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LiveTypeId = new SelectList(db.LiveTypes, "Id", "Name", model.LiveTypeId);
            ViewBag.CloudLiveId = new SelectList(db.CloudLives, "activityId", "activityName", model.CloudLiveId);
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", model.EventId);
            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", model.ShopId);
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
