using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore;

namespace Park.Mgt.Models
{
    public class ParkMgtContext : DbContext
    {
        public ParkMgtContext(DbContextOptions<ParkMgtContext> options) : base(options)
        {
        }

        public DbSet<Config> Configs { get; set; }
        public DbSet<Dept> Depts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Online> Onlines { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Power> Powers { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleUser> RoleUsers { get; set; }
        public DbSet<TitleUser> TitleUsers { get; set; }
        public DbSet<RolePower> RolePowers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // https://docs.microsoft.com/en-us/ef/core/modeling/relationships
            // Many-to-many relationships without an entity class to represent the join table are not yet supported.
            // 多对多：不支持没有实体类来表示联接表的多对多关系。
            modelBuilder.Entity<RoleUser>()
                .ToTable("RoleUsers")
                .HasKey(t => new { t.RoleID, t.UserID });
            modelBuilder.Entity<RoleUser>()
                .HasOne(u => u.User)
                .WithMany(u => u.RoleUsers)
                .HasForeignKey(u => u.UserID);
            modelBuilder.Entity<RoleUser>()
               .HasOne(u => u.Role)
               .WithMany(u => u.RoleUsers)
               .HasForeignKey(u => u.RoleID);


            modelBuilder.Entity<TitleUser>()
                .ToTable("TitleUsers")
                .HasKey(t => new { t.TitleID, t.UserID });
            modelBuilder.Entity<TitleUser>()
                .HasOne(u => u.User)
                .WithMany(u => u.TitleUsers)
                .HasForeignKey(u => u.UserID);
            modelBuilder.Entity<TitleUser>()
               .HasOne(u => u.Title)
               .WithMany(u => u.TitleUsers)
               .HasForeignKey(u => u.TitleID);


            modelBuilder.Entity<RolePower>()
                .ToTable("RolePowers")
                .HasKey(t => new { t.RoleID, t.PowerID });
            modelBuilder.Entity<RolePower>()
                .HasOne(u => u.Role)
                .WithMany(u => u.RolePowers)
                .HasForeignKey(u => u.RoleID);
            modelBuilder.Entity<RolePower>()
               .HasOne(u => u.Power)
               .WithMany(u => u.RolePowers)
               .HasForeignKey(u => u.PowerID);

            
            // 自包含
            modelBuilder.Entity<Dept>()
                .HasOne(d => d.Parent)
                .WithMany(d => d.Children)
                .HasForeignKey(d => d.ParentID)
                .IsRequired(false);

            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(d => d.ParentID)
                .IsRequired(false);


            // 一对多
            modelBuilder.Entity<User>()
                .HasOne(u => u.Dept)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DeptID)
                .IsRequired(false);


            // 单个导航属性
            modelBuilder.Entity<Online>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(d => d.UserID)
                .IsRequired();

            modelBuilder.Entity<Menu>()
                .HasOne(m => m.ViewPower)
                .WithMany()
                .HasForeignKey(d => d.ViewPowerID)
                .IsRequired(false);
        }


    }
}