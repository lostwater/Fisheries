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
    public class CelebritiesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Celebrities
        public IQueryable<Celebrity> GetCelebrities()
        {
            return db.Celebrities;
        }

        // GET: api/Celebrities/5
        [ResponseType(typeof(Celebrity))]
        public async Task<IHttpActionResult> GetCelebrity(int id)
        {
            Celebrity celebrity = await db.Celebrities.FindAsync(id);
            if (celebrity == null)
            {
                return NotFound();
            }

            return Ok(celebrity);
        }

        // PUT: api/Celebrities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCelebrity(int id, Celebrity celebrity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != celebrity.Id)
            {
                return BadRequest();
            }

            db.Entry(celebrity).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CelebrityExists(id))
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

        // POST: api/Celebrities
        [ResponseType(typeof(Celebrity))]
        public async Task<IHttpActionResult> PostCelebrity(Celebrity celebrity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Celebrities.Add(celebrity);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = celebrity.Id }, celebrity);
        }

        // DELETE: api/Celebrities/5
        [ResponseType(typeof(Celebrity))]
        public async Task<IHttpActionResult> DeleteCelebrity(int id)
        {
            Celebrity celebrity = await db.Celebrities.FindAsync(id);
            if (celebrity == null)
            {
                return NotFound();
            }

            db.Celebrities.Remove(celebrity);
            await db.SaveChangesAsync();

            return Ok(celebrity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CelebrityExists(int id)
        {
            return db.Celebrities.Count(e => e.Id == id) > 0;
        }
    }
}