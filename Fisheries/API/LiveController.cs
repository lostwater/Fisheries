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
        public IQueryable<Live> GetLive(int page = 0, int pageSize = 100)
        {
            var lcsdk = new Gandalf.LCUploader("nal4hqaahb", "2e44b05a1d3b751efc6a3a3eb1654e79");
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("ver", "3.0");
            args.Add("userid", 823100);
            args.Add("method", "letv.cloudlive.activity.search");

            string retUrl = lcsdk.handleLiveParam("http://api.open.letvcloud.com/live/execute",args);
            string strResult = lcsdk.doRequest(retUrl);
            //return strResult.Replace("\\", "");
            List<Live> list = JsonConvert.DeserializeObject<List<Live>>(strResult);

            return list.Where(l => l.activityStatus != 3).ToList().Skip(page * pageSize).Take(pageSize).AsQueryable();

        }
    }

    public class Live
    {
        public string activityCategory { get; set; }
        public string activityId { get; set; }
        public string activityName { get; set; }
        public int activityStatus { get; set; }
        public string coverImgUrl { get; set; }
        public string createTime { get; set; }
        public string description { get; set; }
        public string endTime { get; set; }
        public int liveNum { get; set; }
        public int needFullView { get; set; }
        public int needIpWhiteList { get; set; }
        public int needRecord { get; set; }
        public int needTimeShift { get; set; }
        public int neededPushAuth { get; set; }
        public int playMode { get; set; }
        public string pushIpWhiteList { get; set; }
        public int pushUrlValidTime { get; set; }
        public string startTime { get; set; }
        public string userCount { get; set; }
    }

}
