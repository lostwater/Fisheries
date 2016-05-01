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
    public class CloudLivesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/CloudLives
        public IQueryable<CloudLive> GetCloudLives()
        {
            return db.CloudLives;
        }

        // GET: api/CloudLives/5
        [ResponseType(typeof(CloudLive))]
        public async Task<IHttpActionResult> GetCloudLive(string id)
        {
            CloudLive cloudLive = await db.CloudLives.FindAsync(id);
            if (cloudLive == null)
            {
                return NotFound();
            }

            return Ok(cloudLive);
        }

        // PUT: api/CloudLives/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCloudLive(string id, CloudLive cloudLive)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cloudLive.activityId)
            {
                return BadRequest();
            }

            db.Entry(cloudLive).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CloudLiveExists(id))
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

        // POST: api/CloudLives
        [ResponseType(typeof(CloudLive))]
        public async Task<IHttpActionResult> PostCloudLive(CloudLive cloudLive)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CloudLives.Add(cloudLive);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CloudLiveExists(cloudLive.activityId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cloudLive.activityId }, cloudLive);
        }

        // DELETE: api/CloudLives/5
        [ResponseType(typeof(CloudLive))]
        public async Task<IHttpActionResult> DeleteCloudLive(string id)
        {
            CloudLive cloudLive = await db.CloudLives.FindAsync(id);
            if (cloudLive == null)
            {
                return NotFound();
            }

            db.CloudLives.Remove(cloudLive);
            await db.SaveChangesAsync();

            return Ok(cloudLive);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CloudLiveExists(string id)
        {
            return db.CloudLives.Count(e => e.activityId == id) > 0;
        }
    }
}