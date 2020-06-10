using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class HomeModel
    {
        public string background { get; set; }
        public string textColor { get; set; }
        public string mainText { get; set; }
        public List<string> subTextList { get; set; }
    }
}