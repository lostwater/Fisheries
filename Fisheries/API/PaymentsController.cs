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
using Pingpp;
using Pingpp.Models;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json.Linq;
using Fisheries.Helper;

namespace Fisheries.API
{
    [RoutePrefix("api/Payments")]
    public class PaymentsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        string error;
        public PaymentsController()
        {
            Pingpp.Pingpp.SetApiKey("sk_live_mzHOiTujLyfTSinr9K840G0K");
            try {
                Pingpp.Pingpp.SetPrivateKeyPath("C:/RSA/rsa_private_key.pem");
            }
            catch(Exception e)
            {
                error = e.ToString();
            }
        }

        Charge PxxRequest(Order order)
        {
            Dictionary<String, String> app = new Dictionary<String, String>();
            app.Add("id", "app_44qbz1PCezv54eT8");
            Dictionary<String, Object> param = new Dictionary<String, Object>();
            param.Add("order_no", order.Id.ToString());
            param.Add("amount", order.OrderPrice*100);
            param.Add("channel", "alipay");
            param.Add("currency", "cny");
            param.Add("subject", order.Event.Name);
            param.Add("body", order.ApplicationUser.Id);
            param.Add("client_ip", "127.0.0.1");
            param.Add("app", app);
            var expired = ((order.OrderTime + new TimeSpan(0,30,0)).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            param.Add("time_expire", expired);
            try
            {
                return Charge.Create(param);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
                return null;
            }

        }

        // GET: api/Payments
        public IQueryable<Payment> GetPayments()
        {
            return db.Payments;
        }

        // GET: api/Payments/5
        [ResponseType(typeof(Payment))]
        public async Task<IHttpActionResult> GetPayment(int id)
        {
            Payment payment = await db.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        [HttpPost]
        [Route("PingReciever")]
        public async Task<IHttpActionResult> PingReciever()
        {
            if(error != null)
            {
                return BadRequest(error);
            }
            var sig = Request.Headers.GetValues("x-pingplusplus-signature").First().ToString();
            var body = await Request.Content.ReadAsStringAsync();
            var result = VerifySignedHash(body, sig, @"C:/RSA/rsa_public_key.pem");
            //if (result != "verify success")
              //  return BadRequest(result);
            var jObject = JObject.Parse(body);
            var type = jObject.SelectToken("type");
            if (type.ToString() == "charge.succeeded" || type.ToString() == "refund.succeeded")
            {
                var refundedAmount = jObject.SelectToken("data.object.amount_refunded");
                var refunded = jObject.SelectToken("data.object.refunded");
                var amount = jObject.SelectToken("data.object.amount"); 
                var channelId = jObject.SelectToken("data.object.transaction_no");
                var orderId = jObject.SelectToken("data.object.order_no");
                var chargeId = jObject.SelectToken("data.object.id");
                var paid = jObject.SelectToken("data.object.paid");
                var order = db.Orders.Find(int.Parse(orderId.ToString()));
                if(order == null)
                    return BadRequest("找不到订单");
                if(order.Payment == null)
                    return BadRequest("找不到支付信息"); 
                if(order.Payment.PingChargeId != chargeId.ToString())
                    return BadRequest("支付信息不一致");
                order.Payment.isPaid = bool.Parse(paid.ToString());
                order.Payment.isRefund = bool.Parse(refunded.ToString());
                order.Payment.PaymentTime = DateTime.Now;
                order.Payment.Amount = decimal.Parse(amount.ToString())*100;
                order.Payment.RefundAmount = decimal.Parse(refundedAmount.ToString())*100;
                order.Payment.ChannelPaymentId = channelId.ToString();
                if (order.Payment.isPaid && !order.Payment.isRefund)
                {
                    order.OrderStatuId = 2;
                    order.Code = GenCode(order);
                   
                }
                if (order.Payment.isPaid && order.Payment.isRefund)
                {
                    order.OrderStatuId = 4;
                }
                await db.SaveChangesAsync();
                IHuiYiSMS.SendOrderCode(order);
                // TODO what you need do
                return Ok();
            }
            else
            {
                // TODO what you need do
                return InternalServerError();
            }
            
        }

        string GenCode(Order order)
        {
            Random rad = new Random();
            var code = rad.Next(1000, 10000).ToString();
            code = DateTime.Now.ToString("yyyyMMdd") + order.PhoneNumber + code;
            return code;
        }
        
        public static string VerifySignedHash(string strDataToVerify, string strSignedData, string strPublicKeyFilePath)
        {
            byte[] signedData = Convert.FromBase64String(strSignedData);

            UTF8Encoding ByteConverter = new UTF8Encoding();
            byte[] dataToVerify = ByteConverter.GetBytes(strDataToVerify);
            try
            {
                string sPublicKeyPem = File.ReadAllText(strPublicKeyFilePath);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider { PersistKeyInCsp = false };
                
                rsa.LoadPublicKeyPEM(sPublicKeyPem);

                if (rsa.VerifyData(dataToVerify, "SHA256", signedData))
                {
                    return "verify success";
                }
                else
                {
                    return "verify fail";
                }

            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return "verify error";
            }

        }

        [HttpPost]
        [Route("Request")]
        public async Task<IHttpActionResult> RequestPayment(int orderId)
        {
            Charge charge;
            var order = await db.Orders.Include(o => o.Payment).Include(o => o.ApplicationUser).Include(o => o.Event).FirstAsync(o => o.Id == orderId);
            if(order == null)
                return BadRequest();
            if((order.OrderTime + new TimeSpan(0,30,0)) < DateTime.Now || order.OrderStatuId != 1)
                return BadRequest();
            if (order.Payment != null || order.PaymentId != null )
            {
                charge = Pingpp.Models.Charge.Retrieve(order.Payment.PingChargeId);
            }
            else
            {
                charge = PxxRequest(order);
                if (charge == null)
                    return BadRequest();

                var payment = new Payment()
                {
                        CreateTime = DateTime.Now,
                        Amount = order.OrderPrice,
                        RefundAmount = 0,
                        isPaid = false,
                        isRefund = false,
                        Channel = "alipay",
                        Description = order.Description,
                        PingChargeId = charge.Id
                    };
                    db.Payments.Add(payment);            
                    await db.SaveChangesAsync();
                    order.PaymentId = payment.Id;
                    order.Payment = payment;
                    await db.SaveChangesAsync();
            }
            return Ok(charge);         
        }


        // PUT: api/Payments/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPayment(int id, Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != payment.Id)
            {
                return BadRequest();
            }

            db.Entry(payment).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
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

        // POST: api/Payments
        [ResponseType(typeof(Payment))]
        public async Task<IHttpActionResult> PostPayment(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            db.Payments.Add(payment);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = payment.Id }, payment);
        }

        // DELETE: api/Payments/5
        [ResponseType(typeof(Payment))]
        public async Task<IHttpActionResult> DeletePayment(int id)
        {
            Payment payment = await db.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            db.Payments.Remove(payment);
            await db.SaveChangesAsync();

            return Ok(payment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentExists(int id)
        {
            return db.Payments.Count(e => e.Id == id) > 0;
        }
    }
}