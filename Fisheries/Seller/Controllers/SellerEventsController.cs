﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fisheries.Models;

namespace Fisheries.Seller.Controllers
{
    public class SellerEventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SellerEvents
        public async Task<ActionResult> Index()
        {
            var events = db.Events.Include(e => e.Shop);
            return View(await events.ToListAsync());
        }

        // GET: SellerEvents/Details/5
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

        // GET: SellerEvents/Create
        public ActionResult Create()
        {
            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name");
            return View();
        }

        // POST: SellerEvents/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,EventFrom,EvenUntil,RegeristFrom,RegeristUntil,Price,Discount,DiscountPrice,StartTime,OxygenTime,BuyPrice,FishType,FishQuantity,Positions,PositionsRemain,Description,Intro,ShopId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", @event.ShopId);
            return View(@event);
        }

        // GET: SellerEvents/Edit/5
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
            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", @event.ShopId);
            return View(@event);
        }

        // POST: SellerEvents/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,EventFrom,EvenUntil,RegeristFrom,RegeristUntil,Price,Discount,DiscountPrice,StartTime,OxygenTime,BuyPrice,FishType,FishQuantity,Positions,PositionsRemain,Description,Intro,ShopId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", @event.ShopId);
            return View(@event);
        }

        // GET: SellerEvents/Delete/5
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

        // POST: SellerEvents/Delete/5
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
