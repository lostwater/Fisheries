using System;
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
using Fisheries.Models;

namespace Fisheries.API
{
    public class LiveTypesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/LiveTypes
        public IQueryable<LiveType> GetLiveTypes()
        {
            return db.LiveTypes;
        }

        // GET: api/LiveTypes/5
        [ResponseType(typeof(LiveType))]
        public async Task<IHttpActionResult> GetLiveType(int id)
        {
            LiveType liveType = await db.LiveTypes.FindAsync(id);
            if (liveType == null)
            {
                return NotFound();
            }

            return Ok(liveType);
        }

        // PUT: api/LiveTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLiveType(int id, LiveType liveType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != liveType.Id)
            {
                return BadRequest();
            }

            db.Entry(liveType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LiveTypeExists(id))
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

        // POST: api/LiveTypes
        [ResponseType(typeof(LiveType))]
        public async Task<IHttpActionResult> PostLiveType(LiveType liveType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LiveTypes.Add(liveType);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = liveType.Id }, liveType);
        }

        // DELETE: api/LiveTypes/5
        [ResponseType(typeof(LiveType))]
        public async Task<IHttpActionResult> DeleteLiveType(int id)
        {
            LiveType liveType = await db.LiveTypes.FindAsync(id);
            if (liveType == null)
            {
                return NotFound();
            }

            db.LiveTypes.Remove(liveType);
            await db.SaveChangesAsync();

            return Ok(liveType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LiveTypeExists(int id)
        {
            return db.LiveTypes.Count(e => e.Id == id) > 0;
        }
    }
}