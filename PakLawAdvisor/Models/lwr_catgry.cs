//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PakLawAdvisor.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class lwr_catgry
    {
        public int lwr_cat_id { get; set; }
        public Nullable<int> lwr_id { get; set; }
        public Nullable<int> cat_id { get; set; }
    
        public virtual law_catagry law_catagry { get; set; }
        public virtual lawyer lawyer { get; set; }
    }
}
