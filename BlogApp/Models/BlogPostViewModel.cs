using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlogApp.Data;

namespace BlogApp.Models
{
    public class BlogPostViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<Comment> Comments { get; set; } 
    }
}