using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Park.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Park.Service.TransactionService;

namespace Park.Service
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
        public static Task<EnterResult> EnterAsync(ParkContext db, string licensePlate, ParkArea parkArea)
        {
            return EnterAsync(db, licensePlate, parkArea, DateTime.Now);
        }
        internal async static Task<EnterResult> EnterAsync(ParkContext db, string licensePlate, ParkArea parkArea, DateTime time)
        {
            //检查是否有空位
            bool hasEmpty = await db.ParkingSpaces.AnyAsync(p => p.ParkArea == parkArea && !p.HasCar);
            //select from parkingspace join parkarea on parkingspace.parkareaid=parkarea.id and not parkingspace.hascar 
            if (!hasEmpty)
            {
                return new EnterResult(false, "停车场已满");
            }
            //获取汽车
            Car car = await GetCarAsync(db, licensePlate, true);
            if (car != null && !car.Enabled)//是否被禁止进入
            {
                return new EnterResult(false, "该车辆禁止进入");
            }
            //新增进出记录
            ParkRecord parkRecord = new ParkRecord()
            {
                Car = car,
                EnterTime = time,
                ParkArea = parkArea
            };
            db.ParkRecords.Add(parkRecord);
            await db.SaveChangesAsync();
            return new EnterResult(true); ;
        }
        /// <summary>
        /// 离开停车场
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="licensePlate">识别到的车牌号</param>
        /// <param name="parkArea">停车区</param>
        /// <returns>离开结果</returns>
        public static Task<LeaveResult> LeaveAsync(ParkContext db, string licensePlate, ParkArea parkArea)
        {
            return LeaveAsync(db, licensePlate, parkArea, DateTime.Now);
        }
        internal async static Task<LeaveResult> LeaveAsync(ParkContext db, string licensePlate, ParkArea parkArea, DateTime time)
        {
            ParkRecord parkRecord=null;
            try
            {
                LeaveResult leave = new LeaveResult() { CanLeave = true };
                Car car = await GetCarAsync(db, licensePlate, false);
                if (car == null)
                {
                    //找不到车，就直接放行，省得麻烦
                    return leave;
                }
                //获取停车记录
                 parkRecord = await db.ParkRecords
                   .OrderByDescending(p => p.EnterTime)
                   .FirstOrDefaultAsync(p => p.Car == car);
                if (parkRecord == null)
                {
                    //找不到记录，就直接放行，省得麻烦
                    return leave;
                }
                leave.ParkRecord = parkRecord;
                //补全进出记录
                parkRecord.LeaveTime = time;
                db.Entry(parkRecord).State = EntityState.Modified;
                CarOwner owner = car.CarOwner;
                //获取价格策略
                var priceStrategy = parkArea.PriceStrategy;
                if (priceStrategy == null)
                {
                    //免费停车场
                    return leave;
                }

                switch (owner)
                {
                    case CarOwner _ when owner.IsFree:
                        //免费用户
                        break;
                    case CarOwner _:
                        if (await IsMonthlyCardValidAsync(db, owner.ID))
                        {
                            //月租用户，放行
                            break;
                        }
                        else
                        {
                            goto needPay;
                        }
                    case null:
                        //没有注册
                    needPay:
                        //非会员或普通用户
                        double price = GetPrice(priceStrategy, parkRecord.EnterTime, time);
                        double balance = owner == null ? 0 : await GetBalanceAsync(db, owner.ID);
                        //计算价格
                        if (balance - price < 0)
                        {
                            //拒绝驶离，要求付费
                            leave.CanLeave = false;
                            leave.NeedToPay = balance - price;
                            return leave;
                        }
                        //新增扣费记录
                        TransactionRecord transaction = new TransactionRecord()
                        {
                            Time = time,
                            Type = TransactionType.Park,
                            Balance = balance - price,
                            Value = -price,
                            CarOwner = owner,
                        };
                        db.TransactionRecords.Add(transaction);
                        parkRecord.TransactionRecord = transaction;//停车记录绑定交易记录
                        break;

                }
                return leave;
            }
            finally
            {
                await db.SaveChangesAsync();

            }

        }

        /// <summary>
        /// 获取应付的价格
        /// </summary>
        /// <param name="priceStrategy"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private static double GetPrice(PriceStrategy priceStrategy, DateTime startTime, DateTime endTime)
        {
            JObject strategy = JObject.Parse(priceStrategy.StrategyJson);
            switch (strategy["type"].Value<string>())
            {
                case "stepHourBase"://按每小时阶梯定价，目前仅支持这一种
                    var priceArray = strategy["prices"] as IEnumerable<JToken>;
                    Dictionary<double, double> prices = new Dictionary<double, double>();
                    foreach (var item in priceArray)
                    {
                        //循环每一个价格阶层
                        double upper = item["upper"].Value<double>();//最长的市场
                        upper = upper == -1 ? double.PositiveInfinity : upper;
                        double price = item["price"].Value<double>();//这个阶层的价格
                        prices.Add(upper, price);
                    }
                    //此时对价格策略的解析已经完成

                    int hour = (int)Math.Ceiling((endTime - startTime).TotalHours);//停车时长，不足一小时按一小时算
                    double sum = 0;//总价
                    double lastUpper = 0;//上一级阶梯的时长
                    foreach (var upper in prices.Keys.OrderBy(p => p))
                    {
                        if (upper >= hour)//已经到达最大阶梯
                        {
                            sum += (hour - lastUpper) * prices[upper];
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

        /// <summary>
        /// 获取车辆信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="licensePlate"></param>
        /// <param name="autoCreate">若不存在，是否自动创建</param>
        /// <returns></returns>
        private async static Task<Car> GetCarAsync(ParkContext db, string licensePlate, bool autoCreate)
        {
            //根据车牌查询
            Car car = await db.Cars.FirstOrDefaultAsync(p => p.LicensePlate == licensePlate);
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
