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
    
    public partial class SocialMediaLink
    {
        public int id { get; set; }
        public Nullable<long> userID { get; set; }
        public Nullable<int> socialMedia { get; set; }
        public string link { get; set; }
    
        public virtual SocialMedia SocialMedia1 { get; set; }
        public virtual UserInformation UserInformation { get; set; }
    }
}
