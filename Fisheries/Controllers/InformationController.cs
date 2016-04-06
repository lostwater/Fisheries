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
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using PagedList;


namespace Fisheries.Controllers
{
    public class InformationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public InformationController()
        {

        }

        // GET: Information
        public ViewResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var info = db.Information.OrderByDescending(i => i.Id);
            if (User.IsInRole("Seller"))
            {
                var userId = User.Identity.GetUserId();
                info = db.Information.Where(i => i.ApplicationUserId == userId).OrderByDescending(i => i.Id);
            }
            var PageInfo = info.ToPagedList(pageNumber, pageSize);
            ViewBag.PageInfo = PageInfo;
            return View(PageInfo);
        }

        public async Task<ActionResult> Celebrity(int? id)
        {
            return View(await db.Information.Where(i=>i.CelebrityId == id).ToListAsync());
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
            if (User.IsInRole("Seller"))
            {
                ViewBag.InformationTypeId = new SelectList(db.InformationTypes.Where(i => i.Id == 4), "Id", "Name");
            }
            else
            {
                ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name");
            }
            
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
                var userId = User.Identity.GetUserId();
                var information = new Information()
                {
                    Title = model.Title,
                    VideoUrl = model.VideoUrl,
                    Intro = model.Intro,
                    CreatedTime = DateTime.Now,
                    InformationTypeId = model.InformationTypeId,
                    IsPublished = false,
                    ApplicationUserId = userId
                };
                db.Information.Add(information);
                await db.SaveChangesAsync();

                return RedirectToAction("Edit",new { id = information.Id });
            }
            ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name", model.InformationTypeId);
            return View(model);
        }

        public ActionResult CreateForCelebrity(int? id)
        {
            ViewBag.InformationTypeId = new SelectList(db.InformationTypes.Where(i => i.Id == 2), "Id", "Name");
            //ViewBag.ApplicationUserId = new SelectList(db.InformationType.Where(u => u.Roles.Any(r => r.RoleId == roleId)), "Id", "UserName");
            return View();
        }

        // POST: Information/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateForCelebrity(int? id, Information model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var information = new Information()
                {
                    Title = model.Title,
                    VideoUrl = model.VideoUrl,
                    Intro = model.Intro,
                    CreatedTime = DateTime.Now,
                    IsPublished = false,
                    CelebrityId = id,
                    ApplicationUserId = userId
                };
                db.Information.Add(information);
                await db.SaveChangesAsync();

                //var fileName = Path.GetFileName(model.Image.FileName);
                //fileName = DateTime.Now.Ticks.ToString() + fileName;
                var path = Path.Combine("~/InformationFiles/", information.Id.ToString());
                if (!Directory.Exists(Server.MapPath(path)))
                    Directory.CreateDirectory(Server.MapPath(path));
                //model.Image.SaveAs(path);     
                return RedirectToAction("Edit", new { id = information.Id });
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
            var model = information;
            if (model.Content != null)
            {
                var content = model.Content.Replace("</script><script type=\"text/javascript\" src=\"http://yuntv.letv.com/bcloud.js\"></script>"
                    , "&lt;/script&gt;&lt;script type=\"text/javascript\" src=\"http://yuntv.letv.com/bcloud.js\"&gt;&lt;/script&gt;");
                content = content.Replace( "<script", "&lt;script");
                content = content.Replace( "type=\"text/javascript\">", "type=\"text/javascript\"&gt;");
                content = content.Replace("&&","&amp;&amp;");
                model.Content = content;
            }
            //var model = new InformationEditModel(information);
            ViewBag.InformationTypeId = new SelectList(db.InformationTypes, "Id", "Name", model.InformationTypeId);
            return View(model);
        }

        // POST: Information/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Information model)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(information).State = EntityState.Modified;
                Information _information = await db.Information.FindAsync(model.Id);

                var content = "";
                if (model.Content != null)
                {
                    //content = model.Content.Replace("&lt;", "<");
                    content = model.Content.Replace("&lt;/script&gt;&lt;script type=\"text/javascript\" src=\"http://yuntv.letv.com/bcloud.js\"&gt;&lt;/script&gt;",
                        "</script><script type=\"text/javascript\" src=\"http://yuntv.letv.com/bcloud.js\"></script>");
                    content = content.Replace("&lt;script", "<script");
                    content = content.Replace("type=\"text/javascript\"&gt", "type=\"text/javascript\">");
                    content = content.Replace("&amp;&amp;", "&&");
                    content = content.Replace("h&gt;0", "h>0");
                    content = content.Replace("var letvcloud_player_conf = &nbsp;", "var letvcloud_player_conf =  ");
                }
                _information.Content = content;
                _information.Title = model.Title;          
                _information.Intro = model.Intro;
                
                _information.VideoUrl = model.VideoUrl;
                _information.IsPublished = model.IsPublished;

                _information.InformationTypeId = model.InformationTypeId;
                _information.ImageUrl = model.ImageUrl;

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
