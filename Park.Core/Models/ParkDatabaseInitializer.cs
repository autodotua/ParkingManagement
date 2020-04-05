using Microsoft.EntityFrameworkCore;
using Park.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace Park.Core.Models
{
    public static class ParkDatabaseInitializer
    {
        public async static Task InitializeAsync(ParkContext context,
            bool addTestParkAreaDatas = true,
            bool addTestCarDatas = true)
        {
            if (!context.Database.EnsureCreated())
            {
                return;
            }

            List<ParkArea> parkAreas = new List<ParkArea>();
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
}",
                    MonthlyPrice = 120
                };
                context.PriceStrategys.Add(priceStrategy);

                for (int i = 0; i < 3; i++)
                {
                    ParkArea parkArea = new ParkArea()
                    {
                        Name = "停车场" + (i + 1),
                        PriceStrategy = priceStrategy,
                        Length = 100,
                        Width = 50
                    };
                    parkAreas.Add(parkArea);
                    context.ParkAreas.Add(parkArea);
                    for (int j = 0; j < r.Next(50, 100); j++)
                    {
                        context.ParkingSpaces.Add(new ParkingSpace()
                        {
                            ParkArea = parkArea,
                            X = r.Next(0, 50),
                            Y = r.Next(0, 50),
                            Width = 5,
                            Height = 2.5,
                            RotateAngle = r.Next(0, 90)
                        });
                    }
                }
                context.SaveChanges();
                var a = context.ParkAreas.First().ParkingSpaces;
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
                    for(int j=0;j<3;j++)//充值
                    {
                       await TransactionService.RechargeMoneyAsync(context, owner, r.Next(2, 20));
                    }
                    for (int j = 0; j < r.Next(2, 5); j++)//车辆
                    {
                        var car = new Car()
                        {
                            LicensePlate = "浙B" + r.Next(10000, 99999),
                            CarOwner = owner
                        };
                        context.Cars.Add(car);
                        context.SaveChanges();
                        //var a = context.Cars.FirstOrDefault().CarOwner == context.CarOwners.FirstOrDefault(); ;

                        for (int k = 0; k < 3; k++)//进出场信息
                        {
                            await ParkService.EnterAsync(context, car.LicensePlate, parkAreas[r.Next(0, 2)]);
                            await ParkService.LeaveAsync(context, car.LicensePlate, parkAreas[r.Next(0, 2)]);
                        }
                    }
                }

                context.SaveChanges();
            }
        }


    }
}