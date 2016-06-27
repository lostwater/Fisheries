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
    [Authorize]
    [RoutePrefix("api/UserLiveRequests")]
    public class UserLiveRequestsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/UserLiveRequests
        public IQueryable<UserLiveRequest> GetUserLiveRequests()
        {
            return db.UserLiveRequests;
        }

        // GET: api/UserLiveRequests/5
        [ResponseType(typeof(UserLiveRequest))]
        public async Task<IHttpActionResult> GetUserLiveRequest(int id)
        {
            UserLiveRequest userLiveRequest = await db.UserLiveRequests.FindAsync(id);
            if (userLiveRequest == null)
            {
                return NotFound();
            }

            return Ok(userLiveRequest);
        }

        // PUT: api/UserLiveRequests/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserLiveRequest(int id, UserLiveRequest userLiveRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userLiveRequest.Id)
            {
                return BadRequest();
            }

            db.Entry(userLiveRequest).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserLiveRequestExists(id))
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

        // POST: api/UserLiveRequests
        [ResponseType(typeof(UserLiveRequest))]
        public async Task<IHttpActionResult> PostUserLiveRequest(UserLiveRequest userLiveRequest)
        {
            userLiveRequest.State = 0;
            var userId = User.Identity.GetUserId();
            userLiveRequest.ApplicationUserId = userId;
            if (!ModelState.IsValid)
            {
                return BadRequest("提交数据错误"+ModelState);
            }
            if(db.UserLiveRequests.Any(r=>r.ApplicationUserId == userId))
            {
                return BadRequest("重复提交");
            }

            db.UserLiveRequests.Add(userLiveRequest);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = userLiveRequest.Id }, userLiveRequest);
        }

        // DELETE: api/UserLiveRequests/5
        [ResponseType(typeof(UserLiveRequest))]
        public async Task<IHttpActionResult> DeleteUserLiveRequest(int id)
        {
            UserLiveRequest userLiveRequest = await db.UserLiveRequests.FindAsync(id);
            if (userLiveRequest == null)
            {
                return NotFound();
            }

            db.UserLiveRequests.Remove(userLiveRequest);
            await db.SaveChangesAsync();

            return Ok(userLiveRequest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserLiveRequestExists(int id)
        {
            return db.UserLiveRequests.Count(e => e.Id == id) > 0;
        }
    }
}