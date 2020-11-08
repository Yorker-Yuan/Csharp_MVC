using smallWorld.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace smallWorld.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Profile_normal()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Profile_normal(CLogin c)
        {
            var user = from a in (new dbCustomerEntities()).Member
                       where a.fAccount == c.account && a.fPassword == c.password
                       select a;
            return View();
        }
    }
}