using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Fisheries.Models;
using System.IO;
using System;
using System.Web.Helpers;
using System.Collections.Generic;
using PagedList;

namespace Fisheries.Controllers
{

    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;

        public EventsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public EventsController()
        {
           
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Events
        public ViewResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IQueryable<Event> events;
            if (User.IsInRole("Seller"))
            {
                var userId = User.Identity.GetUserId();
                events = db.Events.Where(e => e.Shop.ApplicationUserId == userId).Include(e => e.Shop).OrderByDescending(e => e.EventFrom);
            }
            else 
            {
                events = db.Events.Include(@e => @e.Shop).OrderByDescending(e =>e.EventFrom);
            }
            var PageEvents = events.ToPagedList(pageNumber, pageSize);
            ViewBag.PageEvents = PageEvents;
            return View(PageEvents);
            //return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            if (User.IsInRole("Seller"))
            {
                var userId = User.Identity.GetUserId();
                ViewBag.ShopId = new SelectList(db.Shops.Where(s=>s.ApplicationUserId == userId), "Id", "Name");
            }
            else
            {
                ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name");
            }
            
            return View();
        }

        // POST: Events/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.RegeristFrom = DateTime.Now;
                db.Events.Add(@event);
                await db.SaveChangesAsync();

                return RedirectToAction("Edit", new { id = @event.Id });
            }

            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", @event.ShopId);
            if (User.IsInRole("Seller"))
            {
                ViewBag.ShopId = new SelectList(db.Shops.Where(s=>s.ApplicationUserId == User.Identity.GetUserId()) , "Id", "Name", @event.ShopId);
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("Seller"))
            {
                var userId = User.Identity.GetUserId();
                ViewBag.ShopId = new SelectList(db.Shops.Where(s => s.ApplicationUserId == userId), "Id", "Name", @event.ShopId);
            }
            else
            {
                ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", @event.ShopId);
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.PositionsRemain = @event.Positions;
                var date = @event.EventFrom;
                if (date != null)
                {
                  
                    date = date - new TimeSpan(1, 0, 0, 0, 0);
                }
                @event.RegeristUntil = date;
                if (@event.DiscountPrice == 0)
                {
                    @event.DiscountPrice = @event.Price;
                }
                db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", @event.ShopId);
            return View(@event);
        }

        // GET: SellerEvents/Edit/5
        public async Task<ActionResult> EditPositions(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: SellerEvents/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPositions([Bind(Include = "Id,Positions")] Event @event)
        {
            if (ModelState.IsValid)
            {
                var _event = db.Events.Find(@event.Id);
                var dif = @event.Positions - _event.Positions;
                _event.Positions = @event.Positions;
                _event.PositionsRemain = _event.PositionsRemain + dif;
                //db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Event @event = await db.Events.FindAsync(id);
            db.Events.Remove(@event);
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
