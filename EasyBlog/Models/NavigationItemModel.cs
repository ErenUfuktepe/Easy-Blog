using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class NavigationItemModel
    {
        public int priority { get; set; }
        public string content { get; set; }
        public string sectionName { get; set; }
    }
}