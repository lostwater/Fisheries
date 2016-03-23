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
using System.IO;

namespace Fisheries.Controllers
{
    public class CelebritiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Celebrities
        public async Task<ActionResult> Index()
        {
            return View(await db.Celebrities.ToListAsync());
        }

        // GET: Celebrities/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Celebrity celebrity = await db.Celebrities.FindAsync(id);
            if (celebrity == null)
            {
                return HttpNotFound();
            }
            return View(celebrity);
        }

        // GET: Celebrities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Celebrities/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Celebrity celebrity)
        {
            if (ModelState.IsValid)
            {
                db.Celebrities.Add(celebrity);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(celebrity);
        }

        // GET: Celebrities/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Celebrity celebrity = await db.Celebrities.FindAsync(id);
            if (celebrity == null)
            {
                return HttpNotFound();
            }
            return View(celebrity);
        }

        // POST: Celebrities/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Celebrity celebrity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(celebrity).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(celebrity);
        }

        // GET: Celebrities/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Celebrity celebrity = await db.Celebrities.FindAsync(id);
            if (celebrity == null)
            {
                return HttpNotFound();
            }
            return View(celebrity);
        }

        // POST: Celebrities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Celebrity celebrity = await db.Celebrities.FindAsync(id);
            db.Celebrities.Remove(celebrity);
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
