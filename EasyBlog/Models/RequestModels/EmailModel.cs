using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models.RequestModels
{
    public class EmailModel
    {
        public string toEmail { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}