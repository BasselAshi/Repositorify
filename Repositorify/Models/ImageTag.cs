//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Repositorify.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ImageTag
    {
        public int Id { get; set; }
        public string ImageId { get; set; }
        public string TagId { get; set; }
    
        public virtual Image Image { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
