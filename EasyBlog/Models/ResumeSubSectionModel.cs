using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class ResumeSubSectionModel
    {
        public string header { get; set; }
        public string date { get; set; }
        public string location { get; set; }
        public string explanation { get; set; }
        public List<string> explanationItems { get; set; }
        
    }
}