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
    
    public partial class UserInformation
    {
        public long id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public System.DateTime createdDate { get; set; }
        public System.DateTime modifiedDate { get; set; }
        public System.DateTime lastLoginDate { get; set; }
    
        public virtual UserLogin UserLogin { get; set; }
    }
}
