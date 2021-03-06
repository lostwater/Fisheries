﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Fisheries.Models;
using System.Globalization;

namespace Fisheries.API
{
    [RoutePrefix("api/Events")]
    public class EventsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public EventsController()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            //db.Configuration.
        }

        // GET: api/EventsApi
        public IQueryable<Event> GetEvents(int page = 0, int pageSize = 100, string date = "")
        {
            var events = db.Events.Where(e => e.IsPublished);

            if (!string.IsNullOrEmpty(date))
            {
                try
                {
                    DateTime dt = DateTime.ParseExact(date, "ddMMyyyy", CultureInfo.InvariantCulture);
                    events = events.Where(e => DbFunctions.TruncateTime(e.EventFrom).GetValueOrDefault().Date == DbFunctions.TruncateTime(dt.Date).GetValueOrDefault().Date);
                }
                catch { }
            }
            return events.OrderBy(e => e.EventFrom).Skip(page * pageSize).Take(pageSize).Include(e => e.Shop).Include(e => e.Shop.Live);

        }



        // GET: api/EventsApi/5
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> GetEvent(int id)
        {
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            return Ok(@event);
        }

     

        [HttpGet]
        [Route("EventStatu/{id}")]
        [ResponseType(typeof(EventStatu))]
        public async Task<IHttpActionResult> GetEventStatu(int id)
        {
            EventStatu statu = new EventStatu();
            var userId = User.Identity.GetUserId();
            if ((await db.Events.FindAsync(id)).PositionsRemain == 0)
            {
                statu.isOrderable = false;
                statu.Message = "名额已满";
                return Ok(statu);
            }
            if (await db.Orders.AnyAsync(o => o.EventId == id && o.OrderStatuId != 4 && o.ApplicationUserId == userId))
            {
                statu.isOrderable = false;
                statu.Message = "你已报名";
                return Ok(statu);
            }
            else
            {
                statu.isOrderable = true;
                statu.Message = "我要报名";
                return Ok(statu);
            }
        }


        [HttpGet]
        [Route("isOrdered/{id}")]
        //[ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> isOrdered(int id)
        {
            var theuserid = User.Identity.GetUserId();
            if (await db.Orders.AnyAsync(o => o.EventId == id && o.OrderStatuId != 4 && o.ApplicationUserId == theuserid))
               
                return Ok(true);
            else
                return Ok(false);
        }

        [HttpGet]
        [Route("Ordered/{id}")]
        //[ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> Ordered(int id)
        {   
            var theuserid = User.Identity.GetUserId();
            if (await db.Orders.AnyAsync(o => o.EventId == id && o.OrderStatuId != 4 && o.ApplicationUserId == theuserid))
                return Ok();
            else
                return BadRequest();
        }

        // PUT: api/EventsApi/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEvent(int id, Event @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != @event.Id)
            {
                return BadRequest();
            }

            db.Entry(@event).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EventsApi
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> PostEvent(Event @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Events.Add(@event);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = @event.Id }, @event);
        }

        // DELETE: api/EventsApi/5
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> DeleteEvent(int id)
        {
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            db.Events.Remove(@event);
            await db.SaveChangesAsync();

            return Ok(@event);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EventExists(int id)
        {
            return db.Events.Count(e => e.Id == id) > 0;
        }
    }
}