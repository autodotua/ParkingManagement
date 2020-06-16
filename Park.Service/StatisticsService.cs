using Microsoft.EntityFrameworkCore;
using Park.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park.Service
{
    /// <summary>
    /// 统计服务
    /// </summary>
    public static class StatisticsService
    {
        /// <summary>
        /// 获取最近几天每一天的停车数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static async Task<IDictionary<DateTime, int>> GetRecentDaysParkCount(ParkContext db, int days,bool enter)
        {
            //起始时间
            DateTime earliest = DateTime.Now.AddDays(-days);
            var records  =(await db.ParkRecords
                    .Where(p =>(enter? p.EnterTime:p.LeaveTime) > earliest).ToListAsync())
                    .GroupBy(p => (enter ? p.EnterTime : p.LeaveTime).Date);
            SortedDictionary<DateTime, int> result = new SortedDictionary<DateTime, int>();
            foreach (var day in records)
            {
                result.Add(day.Key, day.Count());
            }

            return result;
        }    
        public static async Task<IDictionary<DateTime, int>> GetRecentHoursParkCount(ParkContext db,bool enter)
        {
            //起始时间
            DateTime today = DateTime.Today;//.AddDays(-2);
            var records = await db.ParkRecords
                    .Where(p => (enter ? p.EnterTime : p.LeaveTime) > today).ToListAsync();
            SortedDictionary<DateTime, int> result = new SortedDictionary<DateTime, int>();
            foreach (var record in records)
            {
                DateTime time = new DateTime(today.Year,
                                             today.Month,
                                             today.Day,
                                             (enter ? record.EnterTime : record.LeaveTime).Hour,
                                             30 * ((enter ? record.EnterTime : record.LeaveTime).Minute / 30),
                                             0);
                if(result.ContainsKey(time))
                {
                    result[time]++;
                }
                else
                {
                    result.Add(time, 1);
                }
            }

            return result;
        }

        public static async Task<ParkStatus> GetParkStatusAsync(ParkContext db)
        {
            ParkStatus status = new ParkStatus();
            status.Total =await db.ParkingSpaces.CountAsync();
            status.Used =await db.ParkingSpaces.CountAsync(p => p.HasCar);
            status.Empty = status.Total - status.Used;
            status.TodayMoney = -(await db.TransactionRecords
                .Where(p => p.Time > DateTime.Today && p.Type == TransactionType.Park)
                .SumAsync(p => p.Value));
            return status;
        }
        public struct ParkStatus { 
            public int Total { get; set; }
            public int Empty { get; set; }
            public int Used { get; set; }
            public double TodayMoney { get; set; }
        }
    }
}
