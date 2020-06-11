using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class BlogModel
    {
        public string header { get; set; }
        public string background { get; set; }
        public List<StoryModel> stories { get; set; }

    }
}