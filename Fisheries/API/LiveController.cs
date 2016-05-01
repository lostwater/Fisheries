using Fisheries.Models;
using Gandalf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fisheries.API
{
    [RoutePrefix("api/Live")]
    public class LiveController : ApiController
    {
        [HttpGet]
        public IQueryable<CloudLive> GetLive(int page = 0, int pageSize = 100)
        {
            var lcsdk = new Gandalf.LCUploader("nal4hqaahb", "2e44b05a1d3b751efc6a3a3eb1654e79");
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("ver", "3.0");
            args.Add("userid", 823100);
            args.Add("method", "letv.cloudlive.activity.search");

            string retUrl = lcsdk.handleLiveParam("http://api.open.letvcloud.com/live/execute",args);
            string strResult = lcsdk.doRequest(retUrl);
            //return strResult.Replace("\\", "");
            List<CloudLive> list = JsonConvert.DeserializeObject<List<CloudLive>>(strResult);

            return list.Where(l => l.activityStatus != 3).ToList().Skip(page * pageSize).Take(pageSize).AsQueryable();

        }

        [HttpGet]
        [Route("LocalLive")]
        public IQueryable<Live> GetLocalLive(int type = 0, int page = 0, int pageSize = 100)
        {
            var lives = new ApplicationDbContext().Lives.Where(l => l.CloudLive.activityStatus != 3);
            if(type != 0)
            {
                lives = lives.Where(l => l.LiveTypeId == type);
            }
            return lives.OrderBy(l=>l.CloudLive.startTime).Skip(page * pageSize).Take(pageSize).AsQueryable();

        }
    }

  

}
