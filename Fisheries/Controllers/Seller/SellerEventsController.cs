﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using Fisheries.Models;
using System.IO;

namespace Fisheries.Seller.Controllers
{
    public class SellerEventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SellerEvents
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var events = db.Events
                .Where(e=>e.Shop.ApplicationUserId == userId)
                .Include(e => e.Shop);
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
            var userId = User.Identity.GetUserId();
            ViewBag.ShopId = new SelectList(db.Shops.Where(s => s.ApplicationUserId == userId), "Id", "Name");
            return View();
        }

        // POST: SellerEvents/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Event @event, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                @event.PositionsRemain = @event.Positions;

                db.Events.Add(@event);
                await db.SaveChangesAsync();

                var fileName = Path.GetFileName(image.FileName);
                var path = "~/Event/" + @event.Id.ToString() + "/";
                //path =;
                if (!Directory.Exists(Server.MapPath(path)))
                    Directory.CreateDirectory(Server.MapPath(path));

                var phyicsPath = Path.Combine(Server.MapPath(path), fileName);
                image.SaveAs(phyicsPath);

                @event.AvatarUrl = Url.Content(Path.Combine(path, fileName));

                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.ShopId = new SelectList(db.Shops, "Id", "Name", @event.ShopId);
            if (User.IsInRole("Seller"))
            {
                var userId = User.Identity.GetUserId();
                ViewBag.ShopId = new SelectList(db.Shops.Where(s => s.ApplicationUserId == userId), "Id", "Name", @event.ShopId);
            }
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

        // POST: SellerEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (ModelState.IsValid)
            {
                Event @event = await db.Events.FindAsync(id);
                if (!db.Orders.Any(o => o.Event.Id == @event.Id && o.OrderStatuId != 4))
                {
                    db.Events.Remove(@event);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "已有预定，无法删除。");
                return View(@event);
            }
            return View();
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
