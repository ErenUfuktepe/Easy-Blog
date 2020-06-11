using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class ResumeModel
    {
        public string header { get; set; }
        public string background { get; set; }
        public List<ResumeSectionModel> resumeSections{ get; set; }

    }
}