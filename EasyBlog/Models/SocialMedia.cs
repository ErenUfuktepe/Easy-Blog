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
    
    public partial class SocialMedia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SocialMedia()
        {
            this.SocialMediaLinks = new HashSet<SocialMediaLink>();
        }
    
        public long id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string color { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SocialMediaLink> SocialMediaLinks { get; set; }
    }
}
