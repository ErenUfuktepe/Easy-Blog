//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EasyBlog.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Story
    {
        public long id { get; set; }
        public Nullable<long> blogID { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string image { get; set; }
    
        public virtual Blog Blog { get; set; }
    }
}
