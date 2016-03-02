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

namespace Fisheries.Seller
{
    public class SellerOrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SellerOrders
        public async Task<ActionResult> Index()
        {
            var orders = db.Orders.Include(o => o.ApplicationUser).Include(o => o.Event).Include(o => o.Payment);
            return View(await orders.ToListAsync());
        }

        // GET: SellerOrders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: SellerOrders/Create
        public ActionResult Create()
        {
            //ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Avatar");
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name");
            ViewBag.PaymentId = new SelectList(db.Payments, "Id", "Description");
            return View();
        }

        // POST: SellerOrders/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OrderTime,OrderPrice,Description,Quantity,OrderStatu,Code,PhoneNumber,EventId,PaymentId,ApplicationUserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Avatar", order.ApplicationUserId);
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", order.EventId);
            ViewBag.PaymentId = new SelectList(db.Payments, "Id", "Description", order.PaymentId);
            return View(order);
        }

        // GET: SellerOrders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Avatar", order.ApplicationUserId);
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", order.EventId);
            ViewBag.PaymentId = new SelectList(db.Payments, "Id", "Description", order.PaymentId);
            return View(order);
        }

        // POST: SellerOrders/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OrderTime,OrderPrice,Description,Quantity,OrderStatu,Code,PhoneNumber,EventId,PaymentId,ApplicationUserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           // ViewBag.ApplicationUserId = new SelectList(db.ApplicationUsers, "Id", "Avatar", order.ApplicationUserId);
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", order.EventId);
            ViewBag.PaymentId = new SelectList(db.Payments, "Id", "Description", order.PaymentId);
            return View(order);
        }

        // GET: SellerOrders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: SellerOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
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
