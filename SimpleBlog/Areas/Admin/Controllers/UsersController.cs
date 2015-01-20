﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Areas.Admin.ViewModels;
using SimpleBlog.Infrastructure;
using NHibernate.Linq;
using SimpleBlog.Models;

namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles="admin")]
    [SelectedTab("users")]
    public class UsersController : Controller
    {
        //
        // GET: /Admin/Users/

        public ActionResult Index()
        {
            return View(new UsersIndex { 
            Users = Database.Session.Query<User>().ToList()
            });
        }

        public ActionResult New()
        {
            return View(new UsersNew { });
        }

        [HttpPost]
        public ActionResult New(UsersNew form)
        {
            if (Database.Session.Query<User>().Any(u => u.Username == form.Username))
                ModelState.AddModelError("Username", "Username must be unique");

            if (!ModelState.IsValid)
                return View(form);

            var user = new User
            {
                Email = form.Email,
                Username = form.Username
            };

            user.SetPassword(form.Password);
            Database.Session.Save(user);

            return RedirectToAction("index");
        }

        public ActionResult Edit(int Id)
        {
            var user = Database.Session.Load<User>(Id);
            if (user == null)
                return HttpNotFound();
            return View(new UsersEdit
            {
            Username = user.Username,
            Email = user.Email
            });
        }

        [HttpPost]
        public ActionResult Edit(int Id, UsersEdit form)
        {
            var user = Database.Session.Load<User>(Id);
            if (user == null)
                return HttpNotFound();
            if (Database.Session.Query<User>().Any(w => w.Username == form.Username && w.Id != Id))
                ModelState.AddModelError("Username", "Username must be unique");

            if (!ModelState.IsValid)
                return View(form);

            user.Username = form.Username;
            user.Email = form.Email;
            Database.Session.Update(user);

            return RedirectToAction("index");
        }

        public ActionResult ResetPassword(int Id)
        {
            var user = Database.Session.Load<User>(Id);
            if (user == null)
                return HttpNotFound();
            return View(new UsersResetPassword
            {
                Username = user.Username,
            });
        }

        [HttpPost]
        public ActionResult ResetPassword(int Id, UsersResetPassword form)
        {
            var user = Database.Session.Load<User>(Id);
            if (user == null)
                return HttpNotFound();

            form.Username = user.Username;

            if (!ModelState.IsValid)
                return View(form);

            user.SetPassword(form.Password);
            Database.Session.Update(user);

            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            var user = Database.Session.Load<User>(Id);
            if (user == null)
                return HttpNotFound();

            Database.Session.Delete(user);
            return RedirectToAction("index");
        }
    }
}
