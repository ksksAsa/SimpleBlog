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

        public ActionResult New()
        {
            return View("form", new PostsForm { 
            IsNew=true
            });
        }

        public ActionResult Edit(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();

            return View("form", new PostsForm
            {
                IsNew=false,
                PostId=id,
                Content=post.Content,
                Slug=post.Slug,
                Title=post.Title
            });
        }

        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Form(PostsForm form)
        {
            if (form.PostId==null)
            {
                form.IsNew = true;
            }

            if (!ModelState.IsValid)
                return View(form);

            Post post;

            if (form.IsNew)
            {
                post = new Post
                {
                    CreatedAt = DateTime.UtcNow,
                    User = Auth.User,
                };
            }
            else {
                post = Database.Session.Load<Post>(form.PostId);
                if (post == null)
                {
                    return HttpNotFound();
                }

                post.UpdatedAt = DateTime.UtcNow;
            }

            post.Title = form.Title;
            post.Slug = form.Slug;
            post.Content = form.Content;

            Database.Session.SaveOrUpdate(post);

            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Trash(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();
            post.DeletedAt = DateTime.UtcNow;
            Database.Session.Update(post);
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();
            
            Database.Session.Delete(post);
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Restore(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();
            post.DeletedAt = null;
            Database.Session.Update(post);
            return RedirectToAction("index");
        }


    }
}
