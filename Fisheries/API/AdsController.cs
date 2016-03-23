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
    [RoutePrefix("api/Ads")]
    public class AdsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/AdsApi
        public IQueryable<Event> GetAds()
        {
            return db.Ads.Select(a => a.Event);
        }

        [HttpGet]
        [Route("Home")]
        public IQueryable<Ad> Home()
        {
            var ads = db.Ads.Where(a => a.AdCat == 1).Include(a => a.Event).Include(a => a.Event.Shop).Include(a => a.Information);
            return ads;
        }

        [HttpGet]
        [Route("FameHall")]
        public IQueryable<Ad> FameHall()
        {
            var ads = db.Ads.Where(a => a.AdCat == 2).Include(a => a.Event).Include(a => a.Event.Shop).Include(a => a.Information);
            return ads;
        }

        [HttpGet]
        [Route("Events")]
        public IQueryable<Ad> Events()
        {
            var ads = db.Ads.Where(a => a.AdCat == 3).Include(a => a.Event).Include(a => a.Event.Shop).Include(a => a.Information);
            return ads;
        }

        [HttpGet]
        [Route("Live")]
        public IQueryable<Ad> Live()
        {
            var ads = db.Ads.Where(a => a.AdCat == 4).Include(a => a.Event).Include(a => a.Event.Shop).Include(a => a.Information);
            return ads;
        }

        // GET: api/AdsApi/5
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> GetAd(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            return Ok(ad);
        }

        // PUT: api/AdsApi/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAd(int id, Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ad.Id)
            {
                return BadRequest();
            }

            db.Entry(ad).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdExists(id))
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

        // POST: api/AdsApi
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> PostAd(Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ads.Add(ad);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = ad.Id }, ad);
        }

        // DELETE: api/AdsApi/5
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> DeleteAd(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            db.Ads.Remove(ad);
            await db.SaveChangesAsync();

            return Ok(ad);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdExists(int id)
        {
            return db.Ads.Count(e => e.Id == id) > 0;
        }
    }
}