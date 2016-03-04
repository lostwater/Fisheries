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
using Microsoft.AspNet.Identity.Owin;

namespace Fisheries.Controllers
{
    public class ShopsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Shops
        public async Task<ActionResult> Index()
        {
            var shops = db.Shops.Include(s => s.ApplicationUser);
            var userId = Request.QueryString["userId"];
            if (!string.IsNullOrEmpty(userId))
                shops = shops.Where(s => s.ApplicationUserId == userId);
            return View(await shops.ToListAsync());
        }

        //public async Task<ActionResult> Index(string id)
        //{
        //    var shops = db.Shops.Where(s=>s.ApplicationUserId == id).Include(s => s.ApplicationUser);
        //    return View(await shops.ToListAsync());
        //}

        // GET: Shops/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shop = await db.Shops.FindAsync(id);
            if (shop == null)
            {
                return HttpNotFound();
            }
            return View(shop);
        }



        // GET: Shops/Create
        public ActionResult Create()
        {
            /*
            string userId = this.Request.QueryString["userId"];
            if (userId == null || userId == "")           
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = userId;
            //return View(shop);
            //ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Avatar");
            */
            var roleId = HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>().Roles.FirstOrDefault(r => r.Name == "Seller").Id;
            var users = db.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId));
            users = users.Where(u => !db.Shops.Any(s => s.ApplicationUserId == u.Id));
            ViewBag.ApplicationUserId = new SelectList(users, "Id", "UserName");
            return View();
            //return View();
        }

        // POST: Shops/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Address,Intro,Description,ApplicationUserId")] Shop shop)
        {
            if (ModelState.IsValid)
            {
                db.Shops.Add(shop);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "UserName", shop.ApplicationUserId);
            return View(shop);
        }

        // GET: Shops/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shop = await db.Shops.FindAsync(id);
            if (shop == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Avatar", shop.ApplicationUserId);
            return View(shop);
        }

        // POST: Shops/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Address,Intro,Description,ApplicationUserId")] Shop shop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shop).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Avatar", shop.ApplicationUserId);
            return View(shop);
        }

        // GET: Shops/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shop = await db.Shops.FindAsync(id);
            if (shop == null)
            {
                return HttpNotFound();
            }
            return View(shop);
        }

        // POST: Shops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Shop shop = await db.Shops.FindAsync(id);
            db.Shops.Remove(shop);
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
