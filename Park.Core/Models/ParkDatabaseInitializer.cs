using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Park.Core.Models
{
    // https://docs.microsoft.com/zh-cn/aspnet/core/data/ef-rp/intro
    public static class ParkDatabaseInitializer
    {
        public static void Initialize(ParkContext context)
        {
            context.Database.EnsureCreated();

            // 已经初始化
            if (context.CarOwners.Any())
            {
                return;
            }
            Random r = new Random();
            for(int i=0;i<20;i++)//车主
            {
                var owner = new CarOwner()
                {
                    Username = "user" + r.Next(0, short.MaxValue),
                    Password = "1234",
                };
                context.CarOwners.Add(owner);

                for(int j=0;j<2;j++)//车辆
                {
                    var car = new Car()
                    {
                        LicensePlate = "浙B" + r.Next(10000, 99999),
                        CarOwner=owner
                    };
                    context.Cars.Add(car);

                    for (int k = 0; k < 5; k++)//进出场信息
                    {

                    }
                }


            }
           

            context.SaveChanges();
        }


    }
}