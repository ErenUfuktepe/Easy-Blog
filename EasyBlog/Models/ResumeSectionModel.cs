using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Models
{
    public class ResumeSectionModel
    {
        public string header { get; set; }
        public List<ResumeSubSectionModel> resumeSubSections { get; set; }

    }
}