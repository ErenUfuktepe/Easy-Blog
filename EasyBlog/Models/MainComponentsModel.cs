using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models.RequestModels
{
    public class MainComponentsModel
    {
        public string logo { get; set; }
        public string title { get; set; }
        public string textColor { get; set; }
        public string hoverColor { get; set; }
        public string titleColor { get; set; }
        public List<SocialMediaModel> socialMediaList { get; set; }
    }
}