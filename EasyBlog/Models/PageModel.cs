using EasyBlog.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class PageModel
    {
        public string template { get; set; }
        public MainComponentsModel mainComponents { get; set; }
        public NavigationModel navigationModel { get; set; }
        public HomeModel home { get; set; }
        public AboutModel about { get; set; }
        public PortfolioModel portfolio { get; set; }
        public BlogModel blog { get; set; }
        public ResumeModel resume { get; set; }
        public ContactModel contact { get; set; }
        public UserInformationModel userInformation { get; set; }

    }
}