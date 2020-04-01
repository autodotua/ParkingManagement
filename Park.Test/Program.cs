using Microsoft.EntityFrameworkCore;
using Park.Core.Helper;
using Park.Core.Models;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Park.Test
{
    class Program
    {
        static Context db;
        async static Task Main(string[] args)
        {
            try
            {
                db = new Context();
                db.Database.EnsureDeleted();
                ParkDatabaseInitializer.Initialize(db, true, true);
                //Console.WriteLine(await db.ParkRecords.CountAsync());
                //await TestTempCarOwnerEnterAndLeaveAsync();
                await TestGeneralCarOwnerEnterAndLeaveAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static async Task TestTempCarOwnerEnterAndLeaveAsync()
        {
            await ParkService.EnterAsync(db, "浙B12345", await db.ParkAreas.FirstAsync());
            await Task.Delay(2000);
            var result = await ParkService.LeaveAsync(db, "浙B12345", await db.ParkAreas.FirstAsync());
            WriteObject(result);
        }

        private static async Task TestGeneralCarOwnerEnterAndLeaveAsync()
        {
            Car car = await db.Cars.FirstAsync();
            await TransactionService.RechargeMoneyAsync(db, car.CarOwner, 100);//模拟充值100元
            await ParkService.EnterAsync(db, car.LicensePlate, await db.ParkAreas.FirstAsync());
            await Task.Delay(2000);
            await ParkService.LeaveAsync(db, car.LicensePlate, await db.ParkAreas.FirstAsync()); 
            WriteObject(await db.ParkRecords.LastOrDefaultRecordAsync(p => p.EnterTime));
            WriteObject((await db.ParkRecords.LastOrDefaultRecordAsync(p => p.EnterTime)).TransactionRecord);

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
