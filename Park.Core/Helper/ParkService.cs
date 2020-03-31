using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Park.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Park.Core.Helper
{
    public static class ParkService
    {
        /// <summary>
        /// 进入停车场
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="licensePlate">识别到的车牌号</param>
        /// <param name="parkArea">停车区</param>
        /// <returns>是否允许进入</returns>
        public async static Task<bool> EnterAsync(ParkContext db, string licensePlate, ParkArea parkArea)
        {
            //检查是否有空位
            bool hasEmpty =await db.ParkingSpaces.AnyAsync(p => p.ParkArea == parkArea);
            if (!hasEmpty)
            {
                return false;
            }
            //获取汽车
            Car car =await GetCarAsync(db, licensePlate, true);
            //新增进出记录
            ParkRecord parkRecord = new ParkRecord()
            {
                Car = car,
                EnterTime = DateTime.Now,
            };
            db.ParkRecords.Add(parkRecord);
            await db.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// 离开停车场
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="licensePlate">识别到的车牌号</param>
        /// <param name="parkArea">停车区</param>
        /// <returns>离开结果</returns>
        public async static Task<LeaveResult> LeaveAsync(ParkContext db, string licensePlate, ParkArea parkArea)
        {
            DateTime leaveTime = DateTime.Now;
            Car car =await GetCarAsync(db, licensePlate, false);
            if (car == null)
            {
                //找不到车，就直接放行，省得麻烦
                return LeaveResult.Go;
            }

            var a =await db.ParkRecords.ToListAsync();
             ParkRecord parkRecord =await db.ParkRecords
                .OrderByDescending(p=>p.EnterTime)
                .FirstOrDefaultAsync(p => p.Car == car);
            if (parkRecord == null)
            {
                //找不到记录，就直接放行，省得麻烦
                return LeaveResult.Go;
            }
            //补全进出记录
            parkRecord.LeaveTime = leaveTime;
            db.Entry(parkRecord).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            CarOwner owner = car.CarOwner;
            if (owner != null && owner.Type == PaymentType.Free)
            {
                //免费用户
                return LeaveResult.Go;
            }

            var priceStrategy = parkArea.PriceStrategy;
            if (priceStrategy == null)
            {
                //免费停车场
                return LeaveResult.Go;
            }

            double price = GetPrice(priceStrategy, parkRecord.EnterTime, leaveTime);
            double balance =await GetBalanceAsync(db, owner);

            if (balance - price < 0)
            {
                return new LeaveResult()
                {
                    CanLeave = false,
                    NeedToPay = balance - price
                };
            }
            TransactionRecord transaction = new TransactionRecord()
            {//新增扣费记录
                Time = leaveTime,
                Type = TransactionType.Park,
                Balance = balance - price,
                Value = -price,
                CarOwner = owner,
            };
            db.TransactionRecords.Add(transaction);
            parkRecord.TransactionRecord = transaction;
            await db.SaveChangesAsync();
            return LeaveResult.Go;
        }

        private async static Task<double> GetBalanceAsync(ParkContext db, CarOwner owner)
        {
            double? balance =(await db.TransactionRecords
                .OrderByDescending(p=>p.Time)
                .FirstOrDefaultAsync(p => p.CarOwner == owner))?.Balance;
            return balance.HasValue ? balance.Value : 0;
        }

        private static double GetPrice(PriceStrategy priceStrategy, DateTime startTime, DateTime endTime)
        {
            JObject strategy = JObject.Parse(priceStrategy.StrategyJson);
            switch (strategy["type"].Value<string>())
            {
                case "stepHourBase":
                    var priceArray = strategy["prices"] as IEnumerable<JToken>;
                    Dictionary<double, double> prices = new Dictionary<double, double>();
                    foreach (var item in priceArray)
                    {
                        double upper = item["upper"].Value<double>();
                        upper = upper == -1 ? double.PositiveInfinity : upper;
                        double price = item["price"].Value<double>();
                        prices.Add(upper, price);
                    }
                    int hour = (int)Math.Ceiling((endTime - startTime).TotalHours);
                    double sum = 0;
                    double lastUpper = 0;
                    foreach (var upper in prices.Keys.OrderBy(p => p))
                    {
                        if (upper >= hour)
                        {//已经到达最大阶梯
                            sum += (sum - lastUpper) * prices[upper];
                            break;
                        }
                        else
                        {
                            sum += (upper - lastUpper) * prices[upper];
                            lastUpper = upper;
                        }
                    }
                    return sum;
                default:
                    throw new NotImplementedException();
            }
        }


        private async static Task<Car> GetCarAsync(ParkContext db, string licensePlate, bool autoCreate)
        {
            Car car =await db.Cars.FirstOrDefaultAsync(p => p.LicensePlate == licensePlate);
            if (!autoCreate)
            {
                return car;
            }
            if (car == null)
            {
                car = new Car()
                {
                    LicensePlate = licensePlate
                };
                db.Cars.Add(car);
            }
            return car;
        }
    }
}
