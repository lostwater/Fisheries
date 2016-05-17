using Fisheries.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Fisheries.Helper
{
    public class LiveSync
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void UpdateChatRoom()
        {
            db.Lives.Where(live=>String.IsNullOrEmpty(live.ChatId)).Where(live=>live.CloudLive.activityStatus!=3).ToList().ForEach(live=>
            {
                var appid = "F9JATEw7PKIUBQ3RObXW7n7y-gzGzoHsz";
                var appkey = "JMggWcALiKHQqkzj10mNuYHF";
                var chatname = live.CloudLive.activityName;
                var client = new RestClient("https://api.leancloud.cn/");
                var request = new RestRequest("1.1/classes/_Conversation");
                request.AddHeader("X-LC-Id", appid);
                request.AddHeader("X-LC-Key", appkey);
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new { name = chatname, tr = true });
                request.Method = Method.POST;
                //request.AddParameter("name", chatname);
                //request.AddParameter("tr", true);
                client.ExecuteAsync(request, response =>
                {
                    var content = response.Content;
                    var result = JsonConvert.DeserializeObject<ChatResult>(content);
                    live.ChatId = result.objectId;

                    db.Entry(live).State = EntityState.Modified;
                    
                    // do something with the response
                });
               
            }
            );
            db.SaveChanges();
        }

        public void UpdateCloudLive()
        {
            var lcsdk = new Gandalf.LCUploader("nal4hqaahb", "2e44b05a1d3b751efc6a3a3eb1654e79");
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("ver", "3.0");
            args.Add("userid", 823100);
            args.Add("method", "letv.cloudlive.activity.search");

            string retUrl = lcsdk.handleLiveParam("http://api.open.letvcloud.com/live/execute", args);
            string strResult = lcsdk.doRequest(retUrl);
            //return strResult.Replace("\\", "");
            List<CloudLive> list = JsonConvert.DeserializeObject<List<CloudLive>>(strResult);
            list = list.Where(l => l.activityStatus != 3).ToList();
            list.ForEach(cl =>
            {
                if (!db.CloudLives.Any(_cl => _cl.activityId == cl.activityId))
                {
                    db.CloudLives.Add(cl);
                }
                else
                {
                    //db.CloudLives.Attach(cl);
                    db.Entry(cl).State = EntityState.Modified;
                }

                if (!db.Lives.Any(l => l.CloudLiveId == cl.activityId))
                {
                    var live = new Live()
                    {
                        CloudLiveId = cl.activityId,
                        CloudLive = cl
                    };
                    db.Lives.Add(live);
                }
            }
            );
            db.SaveChanges();
        }

        public void Sync()
        {
            UpdateCloudLive();
            UpdateChatRoom();
        }

        // GET: Lives/Create")

        public class ChatResult
        {
            public string objectId { get; set; }
            public DateTime createdAt { get; set; }
        }

    }
}