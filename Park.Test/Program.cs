using Microsoft.EntityFrameworkCore;
using Park.Core.Helper;
using Park.Core.Models;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Park.Test
{
    class Program
    {
        async static Task Main(string[] args)
        {
            try
            {
                Context db = new Context();
                db.Database.EnsureDeleted();
                ParkDatabaseInitializer.Initialize(db);
                await ParkService.EnterAsync(db, (await db.Cars.FirstAsync()).LicensePlate, await db.ParkAreas.FirstAsync());
                await Task.Delay(100);
                await ParkService.LeaveAsync(db, (await db.Cars.FirstAsync()).LicensePlate, await db.ParkAreas.FirstAsync());
                await db.SaveChangesAsync();
                //Console.WriteLine(await db.ParkRecords.CountAsync());
                WriteObject(await db.ParkRecords.FirstOrDefaultAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void WriteObject(object obj)
        {
            if (obj == null)
            {
                Console.WriteLine("null");
            }
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                Console.WriteLine("{0}={1}", name, value);
            }
        }

        public class Context : ParkContext
        {
            string connStr = "Data Source=(localdb)\\MSSQLLocalDB;Database=ParkTestContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);
                optionsBuilder.UseSqlServer(connStr);
            }
        }
    }
}
