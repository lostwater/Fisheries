using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fisheries.Models;
using Gandalf;

namespace Fisheries.Controllers
{
    public class VideosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Videos
        public async Task<ActionResult> Index()
        {
            var videos = db.Videos.Include(v => v.Celebrity);
            return View(await videos.ToListAsync());
        }

        // GET: Videos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = await db.Videos.FindAsync(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // GET: Videos/Create
        public ActionResult Create()
        {
            ViewBag.CelebrityId = new SelectList(db.Celebrities, "Id", "Name");
            return View();
        }

        // POST: Videos/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,LCId,CelebrityId")] Video video)
        {
   
            if (ModelState.IsValid)
            {
                try
                {
                    setVideoFromLC(video);
                }
                catch
                { }
                db.Videos.Add(video);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CelebrityId = new SelectList(db.Celebrities, "Id", "Name", video.CelebrityId);
            return View(video);
        }

        void setVideoFromLC(Video video)
        {
            var lcsdk = new Gandalf.LCUploader("nal4hqaahb", "2e44b05a1d3b751efc6a3a3eb1654e79");
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("api", "video.get");
            args.Add("video_id", video.LCId.ToString());
            string retUrl = lcsdk.handleParam(args);
            string strResult = lcsdk.doRequest(retUrl);

            jsonout jsonRes = lcsdk.jsonGetResult(strResult);
            lcsdk.errorCode = int.Parse(jsonRes.code);
            lcsdk.message = jsonRes.message;
            //string total = json.total;
            //string data = json.data.token;
            if (!jsonRes.code.Equals("0"))
            {

            }

            //jsonin json = jsonGetData(jsonRes.data.ToString());
            Dictionary<string, object> data = (Dictionary<string, object>)jsonRes.data;
            video.ImageUrl = data["img"].ToString();
            video.Name = data["video_name"].ToString();
            video.VU = data["video_unique"].ToString();
        }


        // GET: Videos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = await db.Videos.FindAsync(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            ViewBag.CelebrityId = new SelectList(db.Celebrities, "Id", "Name", video.CelebrityId);
            return View(video);
        }

        // POST: Videos/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,LCId,CelebrityId")] Video video)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    setVideoFromLC(video);
                }
                catch
                { }
                db.Entry(video).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CelebrityId = new SelectList(db.Celebrities, "Id", "Name", video.CelebrityId);
            return View(video);
        }

        // GET: Videos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = await db.Videos.FindAsync(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // POST: Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Video video = await db.Videos.FindAsync(id);
            db.Videos.Remove(video);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
