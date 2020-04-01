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
        public DbSet<ParkRecord> ParkRecords { get; set; }
        public DbSet<TransactionRecord> TransactionRecords { get; set; }
        public DbSet<PriceStrategy> PriceStrategys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

    }

}
