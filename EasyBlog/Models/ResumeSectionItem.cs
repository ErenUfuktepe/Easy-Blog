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
    
    public partial class ResumeSectionItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ResumeSectionItem()
        {
            this.ResumeSectionItemExplanations = new HashSet<ResumeSectionItemExplanation>();
        }
    
        public long id { get; set; }
        public Nullable<long> resumeSectionID { get; set; }
        public string header { get; set; }
        public string date { get; set; }
        public string location { get; set; }
        public string explanation { get; set; }
    
        public virtual ResumeSection ResumeSection { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResumeSectionItemExplanation> ResumeSectionItemExplanations { get; set; }
    }
}