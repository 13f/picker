﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Picker.Postgresql
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class pickerEntities : DbContext
    {
        public pickerEntities()
            : base("name=pickerEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<FellowPlusCompany> FellowPlusCompany { get; set; }
        public virtual DbSet<FellowPlusInvest> FellowPlusInvest { get; set; }
        public virtual DbSet<FellowPlusNews> FellowPlusNews { get; set; }
        public virtual DbSet<FellowPlusProject> FellowPlusProject { get; set; }
        public virtual DbSet<FellowPlusProjectPreview> FellowPlusProjectPreview { get; set; }
        public virtual DbSet<FellowPlusWebsite> FellowPlusWebsite { get; set; }
        public virtual DbSet<FellowPlusWeibo> FellowPlusWeibo { get; set; }
        public virtual DbSet<FellowPlusWeixin> FellowPlusWeixin { get; set; }
    }
}
