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
using SimpleBlog.Infrastructure.Extensions;
namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles="admin")]
    [SelectedTab("posts")]
    public class PostsController : Controller
    {
        //
        // GET: /Admin/Posts/
        private const int PostsPerPage = 10;


        public ActionResult Index(int page=1)
        {
            var totalPostsCount = Database.Session.Query<Post>().Count();

            var basicQuery = Database.Session.Query<Post>().OrderByDescending(f => f.CreatedAt);

            var postIds = basicQuery
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .Select(p => p.Id)
                .ToArray();

            var currentPostPage = basicQuery
                .Where(p => postIds.Contains(p.Id))
                .FetchMany(f=>f.Tags)
                .Fetch(f=>f.User)
                .ToList();

            return View(new PostsIndex { 
            Posts = new PageData<Post>(currentPostPage,totalPostsCount,page,PostsPerPage)
            });
        }

        public ActionResult New()
        {
            return View("form", new PostsForm
            {
                IsNew = true,
                Tags = Database.Session.Query<Tag>().Select(tag => new TagCheckBox { 
                Id = tag.Id,
                Name = tag.Name,
                IsChecked = false
                }).ToList()
            });
        }

        public ActionResult Edit(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();

            return View("form", new PostsForm
            {
                IsNew = false,
                PostId = id,
                Content = post.Content,
                Slug = post.Slug,
                Title = post.Title,
                Tags = Database.Session.Query<Tag>().Select(tag => new TagCheckBox { 
                Id=tag.Id,
                Name=tag.Name,
                IsChecked=post.Tags.Contains(tag)
                }).ToList()
            });

            Database.Session.Update(post);
        }

        [HttpPost,ValidateAntiForgeryToken,ValidateInput(false)]
        public ActionResult Form(PostsForm form)
        {
            if (form.PostId==null)
            {
                form.IsNew = true;
            }

            if (!ModelState.IsValid)
                return View(form);

            var selectedTags = ReconsileTags(form.Tags).ToList();

            Post post;

            if (form.IsNew)
            {
                post = new Post
                {
                    CreatedAt = DateTime.UtcNow,
                    User = Auth.User,
                };

                foreach (var tag in selectedTags)
                {
                    post.Tags.Add(tag);
                }
            }
            else {
                post = Database.Session.Load<Post>(form.PostId);
                if (post == null)
                {
                    return HttpNotFound();
                }

                post.UpdatedAt = DateTime.UtcNow;

                foreach (var toAdd in selectedTags.Where(t => !post.Tags.Contains(t)))
                    post.Tags.Add(toAdd);

                foreach (var toRemove in post.Tags.Where(t => !selectedTags.Contains(t)).ToList())
                    post.Tags.Remove(toRemove);
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

        private IEnumerable<Tag> ReconsileTags(IEnumerable<TagCheckBox> tags)
        {
            foreach (var tag in tags.Where(t => t.IsChecked))
            {
                if (tag.Id != null)
                {
                    yield return Database.Session.Load<Tag>(tag.Id);
                    continue;
                }

                var existingTag = Database.Session.Query<Tag>().FirstOrDefault(t => t.Name == tag.Name);
                if (existingTag != null)
                {
                    yield return existingTag;
                    continue;
                }

                var newTag = new Tag {
                Name=tag.Name,
                Slug = tag.Name.Slugify()
                };

                Database.Session.Save(newTag);
                yield return newTag;
            }
        }
    }
}
