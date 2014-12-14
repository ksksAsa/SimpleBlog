using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{
    public class AuthController : Controller
    {
        //
        // GET: /Auth/

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AuthLogin form)
        {
            if(!ModelState.IsValid)
            return View(form);

            if (form.Username != "serafeim")
            {
                ModelState.AddModelError("Username", "username is not cool");
                return View(form);
            }

            return Content("form is valid");
        }
    }
}
