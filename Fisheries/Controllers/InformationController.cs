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
    public class InformationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Information
        public async Task<ActionResult> Index()
        {
            return View(await db.Information.ToListAsync());
        }

        // GET: Information/Details/5
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

        // GET: Information/Create
        public ActionResult Create()
        {
            //ViewBag.ApplicationUserId = new SelectList(db.InformationType.Where(u => u.Roles.Any(r => r.RoleId == roleId)), "Id", "UserName");
            return View();
        }

        // POST: Information/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( InformationCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(model.Image.FileName);
                fileName = DateTime.Now.Ticks.ToString() + fileName;
                var path = Path.Combine(Server.MapPath("~/InformationFile/"), fileName);
                if (!Directory.Exists(Server.MapPath("~/InformationFile/")))
                    Directory.CreateDirectory(Server.MapPath("~/InformationFile/"));
                model.Image.SaveAs(path);

                var information = new Information()
                {
                    Title = model.Title,
                    VideoUrl = model.VideoUrl,
                    Content = model.Content,
                    Time = DateTime.Now,
                    ImageUrl = Path.Combine("./InformationFile/", fileName),
                    InformationTypeId = model.InformationTypeId
                };
                db.Information.Add(information);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Information/Edit/5
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
            return View(information);
        }

        // POST: Information/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,VideoUrl,ImageUrl,Content,Time")] Information information)
        {
            if (ModelState.IsValid)
            {
                db.Entry(information).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(information);
        }

        // GET: Information/Delete/5
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

        // POST: Information/Delete/5
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
