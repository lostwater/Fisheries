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
    public class LiveTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LiveTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.LiveTypes.ToListAsync());
        }

        // GET: LiveTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveType liveType = await db.LiveTypes.FindAsync(id);
            if (liveType == null)
            {
                return HttpNotFound();
            }
            return View(liveType);
        }

        // GET: LiveTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LiveTypes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] LiveType liveType)
        {
            if (ModelState.IsValid)
            {
                db.LiveTypes.Add(liveType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(liveType);
        }

        // GET: LiveTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveType liveType = await db.LiveTypes.FindAsync(id);
            if (liveType == null)
            {
                return HttpNotFound();
            }
            return View(liveType);
        }

        // POST: LiveTypes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] LiveType liveType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liveType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(liveType);
        }

        // GET: LiveTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveType liveType = await db.LiveTypes.FindAsync(id);
            if (liveType == null)
            {
                return HttpNotFound();
            }
            return View(liveType);
        }

        // POST: LiveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LiveType liveType = await db.LiveTypes.FindAsync(id);
            db.LiveTypes.Remove(liveType);
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
