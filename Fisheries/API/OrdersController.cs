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
using Fisheries.Helper;

namespace Fisheries.API
{
    [Authorize(Roles = "Administrator,Seller,Buyer")]
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/BuyerOrders
        [ResponseType(typeof(IQueryable<Order>))]
        public async Task<IHttpActionResult> GetOrders(int timePara = 0, int statuPara = 0)
        {
            if (!User.Identity.IsAuthenticated)
                return BadRequest();
            else
            {
                var userId = User.Identity.GetUserId();
                var orders = db.Orders.Where(o => o.ApplicationUserId == userId);
                if (timePara!=0)
                {
                    var afterTime = new DateTime();
                    if (timePara == 1)
                        afterTime = DateTime.Now - new TimeSpan(30, 0, 0, 0);
                    if (timePara == 2)
                        afterTime = DateTime.Now - new TimeSpan(7, 0, 0, 0);
                   orders = orders.Where(o => o.OrderTime > afterTime);
                }
                if (statuPara != 0)
                {
                    orders = orders.Where(o => o.OrderStatuId == statuPara);
                }

               
                return Ok(await orders.Include(o => o.Event).Include(o => o.Event.Shop).Include(o =>o.Payment).OrderByDescending(o => o.Id).ToListAsync());
            }
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
            order = await db.Orders.Include(o => o.Event).Include(o=>o.Event.Shop).Include(o => o.Payment).FirstAsync(o => o.Id == id);
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
        [Route("CreateOrder/{eventId}")]
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
                return BadRequest("已无剩余钓位");
            }
            //Double Check position
            _event.PositionsRemain = _event.Positions - db.Orders.Count(o => o.EventId == _event.Id && o.OrderStatuId != 4);
            db.SaveChanges();
            if (_event.PositionsRemain == 0)
            {
                return BadRequest("已无剩余钓位");
            }

            var userId = User.Identity.GetUserId();
            if ( db.Orders.Any(o => o.EventId == eventId && o.OrderStatuId != 4 && o.ApplicationUserId == userId))
                return BadRequest("你已报名");
            //User.Identity.GetUserName
            var user = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
            var order = new Order()
            {
                EventId = eventId,
                OrderPrice = _event.DiscountPrice,
                OrderTime = DateTime.Now,
                OrderStatuId = 1,
                Quantity = 1,
                PhoneNumber = user.PhoneNumber,
                ApplicationUserId = User.Identity.GetUserId()
            };
            db.Orders.Add(order);
            _event.PositionsRemain--;
            await db.SaveChangesAsync();

            return Ok(order);
        }

        [Route("SendPayment/{id}")]
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> SendPayment(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var order = db.Orders.Find(id);
            if (order == null)
            {
                return BadRequest();
            }
            order = db.Orders.Include(o => o.Event).First(o => o.Id == id);
            //Check the payment
            if (true)
            {//
                //var user = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
                Random rad = new Random();
                var verifyCode = rad.Next(100000, 1000000).ToString();
                order.Code = verifyCode;
                await IHuiYiSMS.SendVerifyCode(order.PhoneNumber, verifyCode);
                await db.SaveChangesAsync();
            }
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