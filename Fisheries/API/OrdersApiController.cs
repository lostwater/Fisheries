using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Fisheries.Models;
using Microsoft.AspNet.Identity.Owin;

namespace Fisheries.API
{
    [Authorize]
    [RoutePrefix("api/Orders")]
    public class OrdersApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/BuyerOrders
        public IQueryable<Order> GetOrders()
        {
            return db.Orders;
        }

        // GET: api/BuyerOrders/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/BuyerOrders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/BuyerOrders
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        // POST: api/Orders/CreateOrder
        [Route("CreateOrder")]
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> CreateOrder(int eventId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var _event = db.Events.Find(eventId);
            if (_event == null)
            {
                return BadRequest();
            }
            if (_event.PositionsRemain == 0)
            {
                return BadRequest("");
            }
            var user = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
            var order = new Order()
            {
                EventId = eventId,
                OrderPrice = _event.Price,
                OrderTime = DateTime.Now,
                OrderStatuId = 0,
                Quantity = 1,
                PhoneNumber = user.PhoneNumber,
                ApplicationUserId = User.Identity.GetUserId()
            };
            db.Orders.Add(order);
            _event.PositionsRemain--;
            await db.SaveChangesAsync();

            return Ok(order);
        }


        // DELETE: api/BuyerOrders/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}