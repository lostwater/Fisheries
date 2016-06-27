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
    public class UserLiveRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserLiveRequests
        public async Task<ActionResult> Index()
        {
            var userLiveRequests = db.UserLiveRequests.Include(u => u.ApplicationUser);
            return View(await userLiveRequests.ToListAsync());
        }

        // GET: UserLiveRequests/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLiveRequest userLiveRequest = await db.UserLiveRequests.FindAsync(id);
            if (userLiveRequest == null)
            {
                return HttpNotFound();
            }
            return View(userLiveRequest);
        }

        // GET: UserLiveRequests/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Avatar");
            return View();
        }

        // POST: UserLiveRequests/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FullName,CitizenId,LiveName,State,ApplicationUserId")] UserLiveRequest userLiveRequest)
        {
            if (ModelState.IsValid)
            {
                db.UserLiveRequests.Add(userLiveRequest);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Avatar", userLiveRequest.ApplicationUserId);
            return View(userLiveRequest);
        }

        // GET: UserLiveRequests/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLiveRequest userLiveRequest = await db.UserLiveRequests.FindAsync(id);
            if (userLiveRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Avatar", userLiveRequest.ApplicationUserId);
            return View(userLiveRequest);
        }

        // POST: UserLiveRequests/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FullName,CitizenId,LiveName,State,ApplicationUserId")] UserLiveRequest userLiveRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userLiveRequest).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Avatar", userLiveRequest.ApplicationUserId);
            return View(userLiveRequest);
        }

        // GET: UserLiveRequests/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLiveRequest userLiveRequest = await db.UserLiveRequests.FindAsync(id);
            if (userLiveRequest == null)
            {
                return HttpNotFound();
            }
            return View(userLiveRequest);
        }

        // POST: UserLiveRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserLiveRequest userLiveRequest = await db.UserLiveRequests.FindAsync(id);
            db.UserLiveRequests.Remove(userLiveRequest);
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

        public async Task<ActionResult> Unapprove(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLiveRequest userLiveRequest = await db.UserLiveRequests.FindAsync(id);
            userLiveRequest.State = 0;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLiveRequest userLiveRequest = await db.UserLiveRequests.FindAsync(id);
            userLiveRequest.State = 1;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Reject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLiveRequest userLiveRequest = await db.UserLiveRequests.FindAsync(id);
            userLiveRequest.State = 2;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
