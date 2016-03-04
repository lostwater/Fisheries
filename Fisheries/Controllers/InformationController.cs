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
            ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name");
            //ViewBag.ApplicationUserId = new SelectList(db.InformationType.Where(u => u.Roles.Any(r => r.RoleId == roleId)), "Id", "UserName");
            return View();
        }

        // POST: Information/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Information model)
        {
            if (ModelState.IsValid)
            {
                var information = new Information()
                {
                    Title = model.Title,
                    VideoUrl = model.VideoUrl,
                    Intro = model.Intro,
                    CreatedTime = DateTime.Now,
                    InformationTypeId = model.InformationTypeId,
                    IsPublished = false
                };
                db.Information.Add(information);
                await db.SaveChangesAsync();

                //var fileName = Path.GetFileName(model.Image.FileName);
                //fileName = DateTime.Now.Ticks.ToString() + fileName;
                var path = Path.Combine("~/InformationFiles/", information.Id.ToString());
                if (!Directory.Exists(Server.MapPath(path)))
                    Directory.CreateDirectory(Server.MapPath(path));
                //model.Image.SaveAs(path);     
                return RedirectToAction("DetailAndPub", information.Id);
            }
            ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name", model.InformationTypeId);
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
            var model = new InformationEditModel(information);
            ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name", model.InformationTypeId);
            return View(model);
        }

        // POST: Information/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(InformationEditModel model)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(information).State = EntityState.Modified;
                Information _information = await db.Information.FindAsync(model.Id);

                if (model.Image != null)
                {
                    var fileName = Path.GetFileName(model.Image.FileName);
                    var path = "~/InformationFiles/" + model.Id.ToString() + "/ ";
                    path = Server.MapPath(path);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    model.Image.SaveAs(path + fileName);
                    _information.ImageUrl = path + fileName;
                }
                //var content = 
                
                _information.PublishedTime = DateTime.Now;
                _information.Content = model.Content;
                _information.Title = model.Title;          
                _information.Intro = model.Intro;
                
                _information.VideoUrl = model.VideoUrl;
                _information.IsPublished = model.IsPublished;

                _information.InformationTypeId = model.InformationTypeId;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
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
