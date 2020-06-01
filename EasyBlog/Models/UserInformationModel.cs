using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class UserInformationModel
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public System.DateTime createdDate { get; set; }
        public System.DateTime modifiedDate { get; set; }
        public System.DateTime lastLoginDate { get; set; }
    }
}