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
using System.Web.Http.OData;

namespace Fisheries.API
{
    [RoutePrefix("api/Information")]
    public class InformationController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/InformationApi
        [EnableQuery]
        public IQueryable<Information> GetInformation()
        {
            return db.Information.OrderByDescending(i => i.Id).Include(i => i.InformationType).AsQueryable();

        }

        [HttpGet]
        [Route("Type/{typeId}")]
        public IQueryable<Information> InformationsByType(int typeId)
        {
            if(typeId == 0 || typeId == null)
                return db.Information.OrderByDescending(i => i.Id).Include(i => i.InformationType);
            return db.Information.Where(i=>i.InformationTypeId == typeId).OrderByDescending(i => i.Id).Include(i => i.InformationType);

        }



        [HttpGet]
        [Route("CeleberityInfomation")]
        public IQueryable<Information> CeleberityInfomation(int id)
        {
            return  db.Information.Where(i=>i.CelebrityId == id).OrderByDescending(i => i.Id).Include(i => i.InformationType);
        }

        [HttpGet]
        [Route("News")]
        public IQueryable<Information> News()
        {
            return db.Information.Where(i => i.InformationTypeId == 1).OrderByDescending(i => i.Id).Include(i => i.InformationType);

        }

        [HttpGet]
        [Route("Others")]
        public IQueryable<Information> Others()
        {
            return db.Information.Where(i=>i.InformationTypeId!=1).OrderByDescending(i => i.Id).Include(i => i.InformationType);

        }

        // GET: api/InformationApi/5
        [ResponseType(typeof(Information))]
        public async Task<IHttpActionResult> GetInformation(int id)
        {
            Information information = await db.Information.FindAsync(id);
            if (information == null)
            {
                return NotFound();
            }

            return Ok(information);
        }

        // PUT: api/InformationApi/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutInformation(int id, Information information)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != information.Id)
            {
                return BadRequest();
            }

            db.Entry(information).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InformationExists(id))
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

        // POST: api/InformationApi
        [ResponseType(typeof(Information))]
        public async Task<IHttpActionResult> PostInformation(Information information)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Information.Add(information);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = information.Id }, information);
        }

        // DELETE: api/InformationApi/5
        [ResponseType(typeof(Information))]
        public async Task<IHttpActionResult> DeleteInformation(int id)
        {
            Information information = await db.Information.FindAsync(id);
            if (information == null)
            {
                return NotFound();
            }

            db.Information.Remove(information);
            await db.SaveChangesAsync();

            return Ok(information);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool InformationExists(int id)
        {
            return db.Information.Count(e => e.Id == id) > 0;
        }
    }
}