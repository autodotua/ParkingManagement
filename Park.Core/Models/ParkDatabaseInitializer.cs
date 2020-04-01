using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Park.Core.Models
{
    // https://docs.microsoft.com/zh-cn/aspnet/core/data/ef-rp/intro
    public static class ParkDatabaseInitializer
    {
        public static void Initialize(ParkContext context,
            bool addTestParkAreaDatas = true,
            bool addTestCarDatas = true)
        {
            context.Database.EnsureCreated();


            Random r = new Random();
            if (addTestParkAreaDatas)
            {
                PriceStrategy priceStrategy = new PriceStrategy()
                {
                    StrategyJson = @"{
  ""type"": ""stepHourBase"", 
  ""prices"": [
    { 
      ""upper"": 5,
      ""price"": 2
    },
    {
      ""upper"": 12,
      ""price"": 1
    },
    {
      ""upper"": -1,
      ""price"": 0.5
    }
  ]
}",MonthlyPrice=120
                };
                context.PriceStrategys.Add(priceStrategy);
                ParkArea parkArea = new ParkArea()
                {
                    Name = "停车场1",
                    PriceStrategy = priceStrategy
                };
                context.ParkAreas.Add(parkArea);
                for (int i = 0; i < 100; i++)
                {
                    context.ParkingSpaces.Add(new ParkingSpace() { ParkArea = parkArea });
                }
                context.SaveChanges();
            }


            if (addTestCarDatas)
            {
                for (int i = 0; i < 20; i++)//车主
                {
                    var owner = new CarOwner()
                    {
                        Username = "user" + r.Next(0, short.MaxValue),
                        Password = "1234",
                    };
                    context.CarOwners.Add(owner);

                    for (int j = 0; j < 2; j++)//车辆
                    {
                        var car = new Car()
                        {
                            LicensePlate = "浙B" + r.Next(10000, 99999),
                            CarOwner = owner
                        };
                        context.Cars.Add(car);
                        context.SaveChanges();
                        //var a = context.Cars.FirstOrDefault().CarOwner == context.CarOwners.FirstOrDefault(); ;

                        for (int k = 0; k < 5; k++)//进出场信息
                        {

                        }
                    }
                }

                context.SaveChanges();
            }
        }


    }
}