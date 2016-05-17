using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Fisheries.Models;
using Fisheries.Providers;
using Fisheries.Results;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;

using Fisheries.Helper;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Fisheries.API
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountApiController : ApiController
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private const string LocalLoginProvider = "Local";

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private string validPhone { get; set; }
        private string validCode { get; set; }

        public AccountApiController()
        {
        }

        public AccountApiController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return Request.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        [Route("UserDetail")]
        [HttpGet]
        [ResponseType(typeof(ApplicationUser))]
        public IHttpActionResult UserDetail()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            
            if (User.IsInRole("Buyer"))
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Include(u => u.Live).Include(u => u.Live.CloudLive).FirstOrDefault(u => u.Id == userId);
                return Ok(user);
            }
               
            else
                return BadRequest("无法验证用户");
        }

        [Route("LiveRequest")]
        [HttpGet]
        [ResponseType(typeof(UserLiveRequest))]
        public IHttpActionResult LiveRequest()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (User.IsInRole("Buyer"))
            {
                var userId = User.Identity.GetUserId();
                var request = db.UserLiveRequests.FirstOrDefault(r => r.ApplicationUserId == userId);
                if (request != null)
                    return Ok(request);
                else
                    return Ok();
            }

            else
                return BadRequest("无法验证用户");
        }

        [Route("Lives")]
        [HttpGet]
        public IQueryable Lives()
        {
            var userId = User.Identity.GetUserId();
            return db.Users.Find(userId).FollowedLives.AsQueryable();
            //turn db.Users.Where(u => u.Id == userId).Include(u => u.FollowedLives).Include(u => u.FollowedLives.CloudLive).FirstOrDefault(u => u.Id == userId).FollowedLives;
        }

        [Route("Shops")]
        [HttpGet]
        public IQueryable Shops()
        {
            var _db = new ApplicationDbContext();
            _db.Configuration.ProxyCreationEnabled = false;
            _db.Configuration.LazyLoadingEnabled = false;
            var userId = User.Identity.GetUserId();
            var user = _db.Users.Include(u => u.FollowedShops.Select(s=>s.Events)).FirstOrDefault(u => u.Id == userId);
            var shops = user.FollowedShops;
            var events = shops.SelectMany(s => s.Events).Where(e => e.IsPublished);
            return events.AsQueryable();
            //turn db.Users.Where(u => u.Id == userId).Include(u => u.FollowedLives).Include(u => u.FollowedLives.CloudLive).FirstOrDefault(u => u.Id == userId).FollowedLives;
        }

        [Route("IsBuyer")]
        [HttpGet]
        public IHttpActionResult IsBuyer()
        {
            if (User.IsInRole("Buyer"))
                return Ok();
            else
                return BadRequest();
        }

        [Route("IsSeller")]
        [HttpGet]
        public IHttpActionResult IsSeller()
        {
            if (User.IsInRole("Seller"))
                return Ok();
            else
                return BadRequest();
        }


        [Route("ChangeAvatar")]
        [HttpPost]
        public async Task<IHttpActionResult> ChangeAvatar()
        {
            try {
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                string root = HttpContext.Current.Server.MapPath("~/Temps/");
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                var provider = new MultipartFormDataStreamProvider(root);
                // Read the form data.
     
                var task = await Request.Content.ReadAsMultipartAsync(provider);
                var file = provider.FileData.First();
                if (file == null)
                    throw new HttpResponseException(HttpStatusCode.NoContent);
                //return BadRequest("No file");
                if (!IsImage(file))
                    throw new HttpResponseException(HttpStatusCode.ExpectationFailed);
                var orgfilename = GetDeserializedFileName(file);
                var ext = ".jpg";
                try { ext = Path.GetExtension(orgfilename); }
                catch { }
                if(string.IsNullOrEmpty(ext))
                    ext = ".jpg";
                var userId = User.Identity.GetUserId();
                var user = UserManager.FindById(userId);
                var phone = user.PhoneNumber;
                var fileName = phone + ext;
                var path = "~/Avatars/Users/";
                var serverPath = HttpContext.Current.Server.MapPath(path);
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
                var savePath = Path.Combine(path, fileName);
                if (File.Exists(HttpContext.Current.Server.MapPath(savePath)))
                    File.Delete(HttpContext.Current.Server.MapPath(savePath));
                File.Move(file.LocalFileName, HttpContext.Current.Server.MapPath(savePath));

                var db = new ApplicationDbContext();
                var dbuser = db.Users.Find(userId);
                dbuser.Avatar = savePath;
                db.SaveChanges();
                //Stream fileStream = File.Create(HttpContext.Current.Server.MapPath(savePath));
                //file.
                //task.GetStream().CopyTo(fileStream);

                //file.SaveAs(HttpContext.Current.Server.MapPath(savePath));
                return Ok();
            }
            catch(Exception ex)
            {
                LogMessageToFile(ex.ToString());
                return InternalServerError(ex);
            }

           
        }

        [Route("ChangeUsername")]
        [HttpPost]
        public IHttpActionResult ChangeUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest();
            ApplicationDbContext db = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();
            if (db.Users.Any(u=>u.Id != userId && u.UserName == username))
            {
                return BadRequest("用户名已存在");
            }
            var user = db.Users.Find(userId);
            user.UserName = username;
            db.SaveChanges();
            return Ok();


        }

        public string GetTempPath()
        {
            string path = "C:/Logs/";
            return path;
        }

        public void LogMessageToFile(string msg)
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(
                GetTempPath() + "My Log File.txt");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }

        private readonly string[] _imageFileExtensions = { ".jpg", ".png", ".gif", ".jpeg" };
        private bool IsImage(MultipartFileData file)
        {
            return true;
            if (file == null) return false;
            var f1 = false;
            //var f1 = file.ContentType.Contains("image");
            var filename = GetDeserializedFileName(file);
            var f2 = _imageFileExtensions.Any(item => filename.EndsWith(item, StringComparison.OrdinalIgnoreCase));
            return f1 || f2;
        }

        private string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        public string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }



        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("外部登录失败。");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("外部登录已与某个帐户关联。");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(BuyerRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!VerifySMS(model.PhoneNumber, model.VerifyCode))
            {
                return BadRequest("验证码无效");
            }
            if (new ApplicationDbContext().Users.Any(u=>u.PhoneNumber == model.PhoneNumber))
            {
                return BadRequest("该手机已注册");
            }

            var user = new ApplicationUser() { UserName = model.PhoneNumber, PhoneNumber = model.PhoneNumber };
            user.CreatedTime = DateTime.Now;

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!RoleManager.RoleExists("Buyer"))
            {
                RoleManager.Create(new ApplicationRole("Buyer"));
            }
            var role = RoleManager.FindByName("Buyer");
            await UserManager.AddToRoleAsync(UserManager.FindByName(user.UserName).Id, role.Name);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(UserResetPasswordModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!VerifySMS(model.phoneNumber, model.verifyCode))
            {
                return BadRequest("验证码无效");
            }
            var user = new ApplicationDbContext().Users.First(u => u.PhoneNumber == model.phoneNumber);
            if(user == null)
            {
                return BadRequest("找不到用户");
            }

            string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, resetToken, model.password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [Route("SendSMS")]
        public async Task<IHttpActionResult> SendSMS(string phoneNumber)
        {
            Random rad = new Random();
            var verifyCode = rad.Next(1000, 10000).ToString();
            if (await IHuiYiSMS.SendVerifyCode(phoneNumber, verifyCode))
            {
                validCode = verifyCode;
                validPhone = phoneNumber;
                return Ok();
            }
            else
                return BadRequest();
        }

        [AllowAnonymous]
        [Route("VerifySMS")]
        public  bool VerifySMS(string phone, string code)
        {

            WebRequest request = WebRequest.Create("https://webapi.sms.mob.com/sms/verify");
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;

            //allows for validation of SSL certificates 

            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            byte[] bs = Encoding.UTF8.GetBytes("appkey=10141f2a0a77c&amp;phone="+phone+"&amp;zone=86&amp;code=" + code);
            request.Method = "Post";
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            return responseFromServer.Contains("200");
        }

        //for testing purpose only, accept any dodgy certificate... 
        public  bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region 帮助程序

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // 没有可发送的 ModelState 错误，因此仅返回空 BadRequest。
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits 必须能被 8 整除。", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
