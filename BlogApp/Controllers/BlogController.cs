using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BlogApp.Data;
using BlogApp.Models;

namespace BlogApp.Controllers
{
    public class BlogController : Controller
    {
        public ActionResult Index()
        {
            var db = new BlogDb(@"Data Source=.\sqlexpress;Initial Catalog=Blog;Integrated Security=True");
            var posts = db.GetTop5Posts();
            foreach (Post p in posts)
            {
                p.Content = StripHtml(p.Content).Substring(0, 200) + "....";
            }
            var vm = new HomePageViewModel { Posts = posts };
            return View(vm);
        }

        public ActionResult Post(int pid)
        {
            var db = new BlogDb(@"Data Source=.\sqlexpress;Initial Catalog=Blog;Integrated Security=True");
            var post = db.GetPostById(pid);
            var comments = db.GetCommentsForPost(pid);
            var vm = new BlogPostViewModel { Post = post, Comments = comments };
            return View(vm);
        }

        [HttpPost]
        public ActionResult PostComment(int postId, string name, string content)
        {
            var db = new BlogDb(@"Data Source=.\sqlexpress;Initial Catalog=Blog;Integrated Security=True");
            Comment comment = new Comment {PostId = postId, Content = content, Name = name, Date = DateTime.Now};
            db.AddComment(comment);
            return RedirectToAction("Post", new {pid = postId});
        }

        private string StripHtml(string text)
        {
            return Regex.Replace(text, "<.*?>", string.Empty);
        }

    }
}
