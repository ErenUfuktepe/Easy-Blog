using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class NavigationModel
    {
        public string logo { get; set; }
        public string barColor { get; set; }
        public List<NavigationItemModel> navigationItems { get; set; }
    }
}