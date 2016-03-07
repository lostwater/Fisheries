using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Fisheries.Helper
{
    public class IHuiYiSMS
    {
        public static string PostUrl = ConfigurationManager.AppSettings["WebReference.Service.PostUrl"];
        static public async Task<bool> SendVerifyCode(string phoneNumber, string verifyCode)
        {
            string account = "cf_xyxd";
            string password = "xiyuanxiongdi";//密码可以使用明文密码或使用32位MD5加密
   
             //validCode = mobile_code.ToString();
            //validPhone = phoneNumber;
            string content = "您的验证码是：" + verifyCode + " 。请不要把验证码泄露给其他人。";

            //Session["mobile"] = mobile;
            //Session["mobile_code"] = mobile_code;

            string postStrTpl = "account={0}&password={1}&mobile={2}&content={3}";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postData = encoding.GetBytes(string.Format(postStrTpl, account, password, phoneNumber, content));

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(PostUrl);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = postData.Length;

            Stream newStream = await myRequest.GetRequestStreamAsync();
            // Send the data.
            newStream.Write(postData, 0, postData.Length);
            newStream.Flush();
            newStream.Close();

            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            if (myResponse.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

                //Response.Write(reader.ReadToEnd());

                string res = reader.ReadToEnd();
                int len1 = res.IndexOf("</code>");
                int len2 = res.IndexOf("<code>");
                string code = res.Substring((len2 + 6), (len1 - len2 - 6));
                if (code == "2")
                {
                    return true;
                }

                int len3 = res.IndexOf("</msg>");
                int len4 = res.IndexOf("<msg>");
                string msg = res.Substring((len4 + 5), (len3 - len4 - 5));


            }
             return false;

        }
        static public async Task<bool> SendOrderCode(string phoneNumber, string code, Fisheries.Models.Order order)
        {
            string account = "cf_xyxd";
            string password = "xiyuanxiongdi";//密码可以使用明文密码或使用32位MD5加密

            //validCode = mobile_code.ToString();
            //validPhone = phoneNumber;
            string content = "您的消费码是：" + code + " 。消费码与手机号码绑定使用。";
            content += "比赛活动名称：" + order.Event.Name;

            //Session["mobile"] = mobile;
            //Session["mobile_code"] = mobile_code;

            string postStrTpl = "account={0}&password={1}&mobile={2}&content={3}";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postData = encoding.GetBytes(string.Format(postStrTpl, account, password, phoneNumber, content));

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(PostUrl);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = postData.Length;

            Stream newStream = await myRequest.GetRequestStreamAsync();
            // Send the data.
            newStream.Write(postData, 0, postData.Length);
            newStream.Flush();
            newStream.Close();

            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            if (myResponse.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

                //Response.Write(reader.ReadToEnd());

                string res = reader.ReadToEnd();
                int len1 = res.IndexOf("</code>");
                int len2 = res.IndexOf("<code>");
                string _code = res.Substring((len2 + 6), (len1 - len2 - 6));
                if (_code == "2")
                {
                    return true;
                }

                int len3 = res.IndexOf("</msg>");
                int len4 = res.IndexOf("<msg>");
                string msg = res.Substring((len4 + 5), (len3 - len4 - 5));


            }
            return false;

        }
    }
}