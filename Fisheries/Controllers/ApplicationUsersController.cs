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
using Microsoft.AspNet.Identity.Owin;
using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity;

namespace Fisheries.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ApplicationDbContext db = new ApplicationDbContext();
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return  HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

        }

        // GET: ApplicationUsers
        public async Task<ActionResult> Index(string phone)
        {
            //var all = RoleManager.Roles.Where(r => r.Name == "Buyer").Select(r => r.Users).ToList();
            ViewBag.userCount = RoleManager.Roles.Where(r => r.Name == "Buyer").First().Users.Count;
            if (String.IsNullOrEmpty(phone))
                return View();
            else
            {
                var users = UserManager.Users.Where(u => u.PhoneNumber == phone);
                return View(users);
            }
        }

        //
        // GET: /Manage/SetPassword
        [Authorize]
        public ActionResult ResetUserPassowrd(String Id)
        {
            //ViewBag.StatusMessage = Message;
            AdminResetPasswordModel model = new AdminResetPasswordModel();
            if (!User.IsInRole("Administrator"))
            {
                var userId = User.Identity.GetUserId();
                var user = UserManager.FindById(userId);
                if (user != null)
                {
                    model.UserName = user.UserName;
                    model.UserId = userId;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var user = UserManager.FindById(Id);
                    if (user != null)
                    {
                        model.UserName = user.UserName;
                        model.UserId = user.Id;
                    }
                    else
                        return RedirectToAction("ResetUserPassowrd");
                }
                else
                {
                    var userId = User.Identity.GetUserId();
                    var user = UserManager.FindById(userId);
                    if (user != null)
                    {
                        model.UserName = user.UserName;
                        model.UserId = userId;
                    }
                }
            }

            return View(model);
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetUserPassowrd(AdminResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                string resetToken = await UserManager.GeneratePasswordResetTokenAsync(model.UserId);
                IdentityResult passwordChangeResult = await UserManager.ResetPasswordAsync(model.UserId, resetToken, model.NewPassword);

                if (passwordChangeResult.Succeeded)
                {
                    ViewBag.StatusMessage = "重置密码成功";
                    return View(model);
                }
                AddErrors(passwordChangeResult);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        // GET: ApplicationUsers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        // GET: ApplicationUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Avatar,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        // POST: ApplicationUsers/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }
        

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
           
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
