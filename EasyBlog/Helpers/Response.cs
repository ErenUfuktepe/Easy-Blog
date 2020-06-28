using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Helpers
{
    public class Response
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public Response(string message, string type)
        {
            this.Message = message;
            this.Type = type;
        }
    }
}