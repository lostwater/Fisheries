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
        public IQueryable<Information> GetInformation(int page = 0, int pageSize = 100, int typeId = 0, int celebrityId = 0)
        {
            var info = db.Information.Where(i => i.IsPublished).Include(i => i.InformationType);
            if (typeId != 0)
                info = info.Where(i => i.InformationTypeId == typeId);
            if (celebrityId != 0)
                info = info.Where(i => i.CelebrityId == celebrityId);
            var result = info.OrderByDescending(i => i.Id).Skip(page * pageSize).Take(pageSize).ToList();

            return result.Select(i =>
                new Information()
                {
                    Id = i.Id,
                    CreatedTime = i.CreatedTime,
                    PublishedTime = i.PublishedTime,
                    ApplicationUser = i.ApplicationUser,
                    ApplicationUserId = i.ApplicationUserId,
                    Celebrity = i.Celebrity,
                    CelebrityId = i.CelebrityId,
                    InformationType = i.InformationType,
                    InformationTypeId = i.InformationTypeId,
                    Intro = i.Intro,
                    Title = i.Title,
                    ImageUrl = i.ImageUrl,
                    Content = i.Content,
                    IsPublished = i.IsPublished,
                    VideoUrl = i.VideoUrl
                }).ToList().AsQueryable();

        }

        [HttpGet]
        [Route("Type/{typeId}")]
        public IQueryable<Information> InformationsByType(int typeId)
        {
            if(typeId == 0 || typeId == null)
                return db.Information.Where(i=>i.InformationType!= null).Where(i => i.IsPublished).OrderByDescending(i => i.Id).Include(i => i.InformationType);
            return db.Information.Where(i => i.IsPublished).Where(i=>i.InformationTypeId == typeId).OrderByDescending(i => i.Id).Include(i => i.InformationType);

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