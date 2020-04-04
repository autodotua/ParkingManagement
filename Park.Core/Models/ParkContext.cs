using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore;

namespace Park.Core.Models
{
    public class ParkContext : DbContext
    {
        public ParkContext(DbContextOptions<ParkContext> options) : base(options)
        {
        }
        public ParkContext() : base()
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<CarOwner> CarOwners { get; set; }
        public DbSet<ParkArea> ParkAreas { get; set; }
        public DbSet<ParkingSpace> ParkingSpaces { get; set; }
        public DbSet<Aisle> Aisles { get; set; }
        public DbSet<ParkRecord> ParkRecords { get; set; }
        public DbSet<TransactionRecord> TransactionRecords { get; set; }
        public DbSet<PriceStrategy> PriceStrategys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //车与车主
            modelBuilder.Entity<Car>()
                .HasOne(c => c.CarOwner)
                .WithMany(o => o.Cars)
                .HasForeignKey(c => c.CarOwnerID)
                .OnDelete(DeleteBehavior.Cascade);

            //车与停车记录
            modelBuilder.Entity<ParkRecord>()
                .HasOne(p => p.Car)
                .WithMany(c => c.ParkRecords)
                .HasForeignKey(p => p.CarID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //停车区和停车策略
            modelBuilder.Entity<ParkArea>()
                .HasOne(a => a.PriceStrategy)
                .WithMany()
                .HasForeignKey(a => a.PriceStrategyID)
                .OnDelete(DeleteBehavior.Cascade);

            //停车位和停车区
            modelBuilder.Entity<ParkingSpace>()
                .HasOne(s => s.ParkArea)
                .WithMany(a => a.ParkingSpaces)
                .HasForeignKey(s => s.ParkAreaID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            //停车区和过道
            modelBuilder.Entity<Aisle>()
                .HasOne(a => a.ParkArea)
                .WithMany(a => a.Aisles)
                .HasForeignKey(s => s.ParkAreaID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //车主和交易记录
            modelBuilder.Entity<TransactionRecord>()
                .HasOne(t => t.CarOwner)
                .WithMany(o=>o.TransactionRecords)
                .HasForeignKey(t => t.CarOwnerID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //停车区和停车记录
            modelBuilder.Entity<ParkRecord>()
                .HasOne(r=>r.ParkArea)
                .WithMany()
                .HasForeignKey(r=>r.ParkAreaID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }

    }

}
