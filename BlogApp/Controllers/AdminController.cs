using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogApp.Data;

namespace BlogApp.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddPost(string content, string title, string tags)
        {
            var db = new BlogDb(@"Data Source=.\sqlexpress;Initial Catalog=Blog;Integrated Security=True");
            Post post = new Post
            {
                Content = content,
                Title = title,
                Date = DateTime.Now
            };
            db.AddPost(post);
            db.AddTagsToPost(tags, post.Id);

            return RedirectToAction("Index");
        }

    }
}
