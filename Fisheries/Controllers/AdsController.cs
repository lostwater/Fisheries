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

namespace Fisheries.Controllers
{
    public class AdCat
    {
        public int id { get; set; }
        public string name { get; set; }

        static public IEnumerable<AdCat> AppAdCats()
        {
            var cat = new List<AdCat>();
            cat.Add(new AdCat() { id = 1, name = "首页" });
            cat.Add(new AdCat() { id = 2, name = "名人堂" });
            cat.Add(new AdCat() { id = 3, name = "赛事" });
            cat.Add(new AdCat() { id = 4, name = "直播" });
            return cat;
        }
    }

    public class AdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ads
        public async Task<ActionResult> Index()
        {
            var ads = db.Ads.Include(a => a.Event);
            return View(await ads.ToListAsync());
        }

        [HttpGet]
        public async Task<ActionResult> All()
        {
            var ads = db.Ads.Include(a => a.Event);
            return View(await ads.ToListAsync());
        }
        [HttpGet]
        public async Task<ActionResult> Home()
        {
            var ads = db.Ads.Where(a=>a.AdCat==1).Include(a => a.Event);
            return View(await ads.ToListAsync());
        }

        [HttpGet]
        public async Task<ActionResult> FameHall()
        {
            var ads = db.Ads.Where(a => a.AdCat == 2).Include(a => a.Event);
            return View(await ads.ToListAsync());
        }

        [HttpGet]
        public async Task<ActionResult> Events()
        {
            var ads = db.Ads.Where(a => a.AdCat == 3).Include(a => a.Event);
            return View(await ads.ToListAsync());
        }
        [HttpGet]
        public async Task<ActionResult> Live()
        {
            var ads = db.Ads.Where(a => a.AdCat == 4).Include(a => a.Event);
            return View(await ads.ToListAsync());
        }
       


        // GET: Ads/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // GET: Ads/Create
        public ActionResult Create()
        {
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name");
            ViewBag.InformationId = new SelectList(db.Information, "Id", "Title");
            ViewBag.AdCat = new SelectList(AdCat.AppAdCats(), "id", "name");
            return View();
        }

        // POST: Ads/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Ad ad)
        {
            if (ModelState.IsValid)
            {
  
                
                db.Ads.Add(ad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AdCat = new SelectList(AdCat.AppAdCats(), "id", "name",ad.AdCat);
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", ad.EventId);
            ViewBag.InformationId = new SelectList(db.Information, "Id", "Title", ad.InformationId);
            return View(ad);
        }

        public ActionResult HomeCreate()
        {
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name");
            ViewBag.InformationId = new SelectList(db.Information, "Id", "Title");
            ViewBag.AdCat = new SelectList(AdCat.AppAdCats(), "id", "name");
            return View();
        }

        // POST: Ads/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HomeCreate(Ad ad)
        {
            if (ModelState.IsValid)
            {


                db.Ads.Add(ad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", ad.EventId);
            ViewBag.InformationId = new SelectList(db.Information, "Id", "Title", ad.InformationId);
            return View(ad);
        }

        // GET: Ads/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdCat = new SelectList(AdCat.AppAdCats(), "id", "name",ad.AdCat);
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", ad.EventId);
            ViewBag.InformationId = new SelectList(db.Information, "Id", "Title", ad.InformationId);
            return View(ad);
        }

        // POST: Ads/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( Ad ad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AdCat = new SelectList(AdCat.AppAdCats(), "id", "name", ad.AdCat);
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", ad.EventId);
            ViewBag.InformationId = new SelectList(db.Information, "Id", "Title", ad.InformationId);
            return View(ad);
        }

        // GET: Ads/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: Ads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            db.Ads.Remove(ad);
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
