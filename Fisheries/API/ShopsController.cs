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
using Microsoft.AspNet.Identity;

namespace Fisheries.API
{
    [RoutePrefix("api/Shops")]
    public class ShopsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Buyer")]
        [HttpPost]
        [Route("Follow/{id}")]
        public async Task<IHttpActionResult> Follow(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            Shop shop = await db.Shops.FindAsync(id);
            if (shop == null)
            {
                return NotFound();
            }
            user.FollowedShops.Add(shop);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Buyer")]
        [HttpPost]
        [Route("Unfollow/{id}")]
        public async Task<IHttpActionResult> Unfollow(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            Shop shop = user.FollowedShops.FirstOrDefault(s => s.Id == s.Id);
            if (shop == null)
            {
                return Ok();
            }
            user.FollowedShops.Remove(shop);
            await db.SaveChangesAsync();
            return Ok();
        }

        // GET: api/Shops
        public IQueryable<Shop> GetShops()
        {
            return db.Shops;
        }

        // GET: api/Shops/5
        [ResponseType(typeof(Shop))]
        public async Task<IHttpActionResult> GetShop(int id)
        {
            Shop shop = await db.Shops.FindAsync(id);
            if (shop == null)
            {
                return NotFound();
            }

            return Ok(shop);
        }


        // PUT: api/Shops/5
        [Authorize(Roles = "Administrator")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutShop(int id, Shop shop)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shop.Id)
            {
                return BadRequest();
            }

            db.Entry(shop).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopExists(id))
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

        // POST: api/Shops
        [Authorize(Roles = "Administrator")]
        [ResponseType(typeof(Shop))]
        public async Task<IHttpActionResult> PostShop(Shop shop)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Shops.Add(shop);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = shop.Id }, shop);
        }

        // DELETE: api/Shops/5
        [Authorize(Roles = "Administrator")]
        [ResponseType(typeof(Shop))]
        public async Task<IHttpActionResult> DeleteShop(int id)
        {
            Shop shop = await db.Shops.FindAsync(id);
            if (shop == null)
            {
                return NotFound();
            }

            db.Shops.Remove(shop);
            await db.SaveChangesAsync();

            return Ok(shop);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShopExists(int id)
        {
            return db.Shops.Count(e => e.Id == id) > 0;
        }
    }
}