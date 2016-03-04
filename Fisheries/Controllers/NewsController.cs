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
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: News
        public async Task<ActionResult> Index()
        {
            var information = db.Information.Include(i => i.InformationType);
            return View(await information.ToListAsync());
        }

        // GET: News/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Information information = await db.Information.FindAsync(id);
            if (information == null)
            {
                return HttpNotFound();
            }
            return View(information);
        }

        // GET: News/Create
        public ActionResult Create()
        {
            ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name");
            return View();
        }

        // POST: News/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,VideoUrl,ImageUrl,Content,Time,InformationTypeId")] Information information)
        {
            if (ModelState.IsValid)
            {
                db.Information.Add(information);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name", information.InformationTypeId);
            return View(information);
        }

        // GET: News/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Information information = await db.Information.FindAsync(id);
            if (information == null)
            {
                return HttpNotFound();
            }
            ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name", information.InformationTypeId);
            return View(information);
        }

        // POST: News/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,VideoUrl,ImageUrl,Content,Time,InformationTypeId")] Information information)
        {
            if (ModelState.IsValid)
            {
                db.Entry(information).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name", information.InformationTypeId);
            return View(information);
        }

        // GET: News/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Information information = await db.Information.FindAsync(id);
            if (information == null)
            {
                return HttpNotFound();
            }
            return View(information);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Information information = await db.Information.FindAsync(id);
            db.Information.Remove(information);
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
