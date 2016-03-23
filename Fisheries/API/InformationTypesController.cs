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
    public class InformationTypesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/InformationTypes
        public IQueryable<InformationType> GetInformationTypes()
        {
            return db.InformationTypes;
        }

        // GET: api/InformationTypes/5
        [ResponseType(typeof(InformationType))]
        public async Task<IHttpActionResult> GetInformationType(int id)
        {
            InformationType informationType = await db.InformationTypes.FindAsync(id);
            if (informationType == null)
            {
                return NotFound();
            }

            return Ok(informationType);
        }

        // PUT: api/InformationTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutInformationType(int id, InformationType informationType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != informationType.Id)
            {
                return BadRequest();
            }

            db.Entry(informationType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InformationTypeExists(id))
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

        // POST: api/InformationTypes
        [ResponseType(typeof(InformationType))]
        public async Task<IHttpActionResult> PostInformationType(InformationType informationType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.InformationTypes.Add(informationType);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = informationType.Id }, informationType);
        }

        // DELETE: api/InformationTypes/5
        [ResponseType(typeof(InformationType))]
        public async Task<IHttpActionResult> DeleteInformationType(int id)
        {
            InformationType informationType = await db.InformationTypes.FindAsync(id);
            if (informationType == null)
            {
                return NotFound();
            }

            db.InformationTypes.Remove(informationType);
            await db.SaveChangesAsync();

            return Ok(informationType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool InformationTypeExists(int id)
        {
            return db.InformationTypes.Count(e => e.Id == id) > 0;
        }
    }
}