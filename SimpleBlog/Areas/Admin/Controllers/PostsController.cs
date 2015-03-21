using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Infrastructure;
using System.Linq;
using NHibernate.Linq;
using SimpleBlog.Models;
using SimpleBlog.Areas.Admin.ViewModels;
namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles="admin")]
    [SelectedTab("posts")]
    public class PostsController : Controller
    {
        //
        // GET: /Admin/Posts/
        private const int PostsPerPage = 5;


        public ActionResult Index(int page=1)
        {
            var totalPostsCount = Database.Session.Query<Post>().Count();
            var currentPostPage = Database.Session.Query<Post>()
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * PostsPerPage)
                .ToList();

            return View(new PostsIndex { 
            Posts = new PageData<Post>(currentPostPage,totalPostsCount,page,PostsPerPage)
            });
        }

    }
}
