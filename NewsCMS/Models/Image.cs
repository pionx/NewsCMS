//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewsCMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Image
    {
        public int ID { get; set; }
        [Required]
        public int NewsID { get; set; }
        [Required]
        public byte[] ImageFile { get; set; }
        [Required]
        public string Ext { get; set; }
    
        public virtual News News { get; set; }
    }
}