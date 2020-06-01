using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models.RequestModels
{
    public class UserLoginModel
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}