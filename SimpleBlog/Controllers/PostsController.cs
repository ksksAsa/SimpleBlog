using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using SimpleBlog.Models;
using SimpleBlog.ViewModels;
using SimpleBlog.Infrastructure;
using System.Text.RegularExpressions;

namespace SimpleBlog.Controllers
{
    public class PostsController : Controller
    {
        //
        // GET: /Posts/
        private const int postsPerPage = 5;

        public ActionResult Index(int page=1)
        {
            var baseQuery = Database.Session.Query<Post>().Where(t => t.DeletedAt == null).OrderByDescending(t => t.CreatedAt);
            var totalPostCount = baseQuery.Count();
            var postIds = baseQuery.Skip((page - 1) * postsPerPage).Take(postsPerPage).Select(t => t.Id).ToArray();
            var posts = baseQuery.Where(t => postIds.Contains(t.Id)).FetchMany(t => t.Tags).Fetch(t => t.User).ToList();

            return View(new PostsIndex { 
            Posts = new PagedData<Post>(posts,totalPostCount,page,postsPerPage)
            });
        }

        public ActionResult Tag(string idAndSlug,int page=1)
        {
            var parts = SeparateIdAndSlug(idAndSlug);
            if (parts == null)
                return HttpNotFound();

            var tag = Database.Session.Load<Tag>(parts.Item1);
            if (tag == null)
                return HttpNotFound();

            if (!tag.Slug.Equals(parts.Item2, StringComparison.CurrentCultureIgnoreCase))
                return RedirectToRoutePermanent("tag", new { id = parts.Item1, slug = tag.Slug });

            var totalPostsCount = tag.Posts.Count;
            var postIds = tag.Posts
                .OrderByDescending(g=>g.CreatedAt)
                .Skip((page - 1) * postsPerPage)
                .Take(postsPerPage)
                .Where(t => t.DeletedAt == null)
                .Select(t => t.Id)
                .ToArray();


            var posts = Database.Session.Query<Post>()
                .OrderByDescending(b => b.CreatedAt)
                .Where(t => postIds.Contains(t.Id))
                .FetchMany(t => t.Tags)
                .Fetch(f => f.User)
                .ToList();

            return View(new PostsTag { 
            Tag=tag,
            Posts = new PagedData<Post>(posts,totalPostsCount,page,postsPerPage)
            
            });

        }

        public ActionResult Show(string idAndSlug)
        {
            var parts = SeparateIdAndSlug(idAndSlug);
            if (parts == null)
                return HttpNotFound();

            var post = Database.Session.Load<Post>(parts.Item1);
            if (post == null)
                return HttpNotFound();

            if (!post.Slug.Equals(parts.Item2, StringComparison.CurrentCultureIgnoreCase))
                return RedirectToRoutePermanent("Post", new { id = parts.Item1, slug = post.Slug });

            return View(new PostShow
            {
                Post = post
            });
        }

        private System.Tuple<int,string> SeparateIdAndSlug(string idAndSlug)
        {
            var matches = Regex.Match(idAndSlug, @"^(\d+)\-(.*)?$");
            if (!matches.Success)
                return null;

            var id= int.Parse(matches.Result("$1"));
            var slug = matches.Result("$2");
            return Tuple.Create(id, slug);
        }

    }
}
