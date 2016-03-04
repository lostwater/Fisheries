using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Fisheries.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction("Index", "Account");
            }
            if (User.IsInRole("Seller"))
            {
                return RedirectToAction("Index","SellerPortal");
            }
            return View();
        }
    }
}
