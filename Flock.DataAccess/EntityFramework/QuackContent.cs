//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Flock.DataAccess.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class QuackContent
    {
        public QuackContent()
        {
            this.Quacks = new HashSet<Quack>();
        }
    
        public int ID { get; set; }
        public string MessageText { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public byte[] Image { get; set; }
        public string ImageUrl { get; set; }
    
        public virtual ICollection<Quack> Quacks { get; set; }
    }
}
