﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Picker.Postgresql
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DoubanEntities : DbContext
    {
        public DoubanEntities()
            : base("name=DoubanEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BookTask> BookTask { get; set; }
        public virtual DbSet<MovieTask> MovieTask { get; set; }
        public virtual DbSet<MusicTask> MusicTask { get; set; }
        public virtual DbSet<TravelTask> TravelTask { get; set; }
        public virtual DbSet<UserTask> UserTask { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Movie> Movie { get; set; }
        public virtual DbSet<Music> Music { get; set; }
        public virtual DbSet<Travel> Travel { get; set; }
    }
}