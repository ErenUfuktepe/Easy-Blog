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
    
    public partial class NavigationItem
    {
        public long id { get; set; }
        public Nullable<long> navID { get; set; }
        public string content { get; set; }
        public string sectionName { get; set; }
        public int priority { get; set; }
    
        public virtual Navigation Navigation { get; set; }
    }
}
