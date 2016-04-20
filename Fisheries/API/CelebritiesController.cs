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
    [RoutePrefix("api/Celebrities")]
    public class CelebritiesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Celebrities
        public IQueryable<Celebrity> GetCelebrities(int page = 0, int pageSize = 100)
        {
            return db.Celebrities.ToList().Skip(page * pageSize).Take(pageSize).AsQueryable();
            //.Include(c=>c..Skip(page * pageSize).Take(pageSize);
        }


       [HttpGet]
       [ResponseType(typeof(IQueryable<Video>))]
       [Route("{id}/Videos")]
        public async Task<IHttpActionResult> Videos(int id, int page = 0, int pageSize = 100)
        {
            Celebrity celebrity = await db.Celebrities.FindAsync(id);
            if (celebrity == null)
            {
                return NotFound();
            }
            var videos =  db.Videos.Where(v => v.CelebrityId == id).OrderByDescending(v => v.Id).Skip(page * pageSize).Take(pageSize);
            return Ok(videos);
        }

        [HttpGet]
        [ResponseType(typeof(List<Information>))]
        [Route("{id}/Information")]
        public async Task<IHttpActionResult> Information(int id)
        {
            Celebrity celebrity = await db.Celebrities.FindAsync(id);
            if (celebrity == null)
            {
                return NotFound();
            }
            var information = await db.Information.Where(v => v.CelebrityId == id).ToListAsync();
            return Ok(information);
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