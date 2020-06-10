using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class AboutModel
    {
        public string image { get; set; }
        public string background { get; set; }
        public string frame { get; set; }
        public string header { get; set; }
        public string body { get; set; }
        public string subTitle { get; set; }
        public List<List<string>> informationList { get; set; }

    }
}