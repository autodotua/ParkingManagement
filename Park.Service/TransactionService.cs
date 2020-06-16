using Microsoft.EntityFrameworkCore;
using Park.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park.Service
{
    /// <summary>
    /// 交易服务
    /// </summary>
    public static class TransactionService
    {
        /// <summary>
        /// 充时长（月租续费）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="owner"></param>
        /// <param name="parkArea"></param>
        /// <param name="month"></param>
        /// <returns>返回交易记录，如果余额不足，则返回null</returns>
        public async static Task<TransactionRecord> RechargeTimeAsync(ParkContext db, int ownerID, int month)
        {
           int price =int.Parse(await Config.GetAsync(db, "MonthlyPrice", "120"));
            Debug.Assert(month > 0);
            double balance = await GetBalanceAsync(db, ownerID);//用户余额
            if(balance<price*month)//如果余额不足，充值随便
            {
                return null;
            }
            DateTime? time = await GetExpireTimeAsync(db, ownerID);//获取到期时间
            DateTime target;
            if(time.HasValue&&time.Value>DateTime.Now)//如果到期时间在当前时间之后
            {
                //那么直接到期时间加上几个月
                target = time.Value.AddMonths(month);
            }
            else
            {
                //否则从现在开始算，加上几个月
                target = DateTime.Now.AddMonths(month);
            }
            //生成交易信息
            TransactionRecord transactionRecord = new TransactionRecord()
            {
                Balance = balance- price * month,
                CarOwnerID = ownerID,
                Time = DateTime.Now,
                Type = TransactionType.RechargeTime,
                Value = -price * month,
                ExpireTime=target
            };
            db.TransactionRecords.Add(transactionRecord);
            await db.SaveChangesAsync();
            return transactionRecord;
        }
        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ownerID"></param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        public  static Task<TransactionRecord> RechargeMoneyAsync(ParkContext db, int ownerID, double amount)
        {
            return RechargeMoneyAsync(db, ownerID, amount, DateTime.Now);
        }
        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ownerID"></param>
        /// <param name="amount"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        internal async static Task<TransactionRecord> RechargeMoneyAsync(ParkContext db,int ownerID,double amount,DateTime time)
        {
            Debug.Assert(amount > 0);
            double balance = await GetBalanceAsync(db, ownerID);//余额
            TransactionRecord transactionRecord = new TransactionRecord()
            {
                Balance=balance+amount,//重新计算余额
                CarOwnerID=ownerID,
                Time= time,
                Type=TransactionType.RechargeMoney,
                Value=amount
            };
            db.TransactionRecords.Add(transactionRecord);
            await db.SaveChangesAsync();
            return transactionRecord;
        }
        /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ownerID"></param>
        /// <returns></returns>
        public async static Task<double> GetBalanceAsync(ParkContext db, int ownerID)
        {
            //获取该用户的最后一条交易记录，来获得余额
            double? balance = (await db.TransactionRecords
                .LastOrDefaultRecordAsync(p => p.Time, p => p.CarOwnerID == ownerID))?.Balance;
            return balance ?? 0;//如果没有交易记录，那么就是0元
        }
        /// <summary>
        /// 是否属于月租时间内
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ownerID"></param>
        /// <returns></returns>
        public async static Task<bool> IsMonthlyCardValidAsync(ParkContext db, int ownerID)
        {
            DateTime? expireTime = await GetExpireTimeAsync(db, ownerID);
            return expireTime.HasValue && expireTime >= DateTime.Now ;
        }
        /// <summary>
        /// 获取月租到期时间
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ownerID"></param>
        /// <returns></returns>
        private async static  Task<DateTime?> GetExpireTimeAsync(ParkContext db, int ownerID)
        {
            //获取最后一条月租续费的交易记录
            var record = await db.TransactionRecords
                .Where(p => p.Type == TransactionType.RechargeTime)//充时记录
                .LastOrDefaultRecordAsync(p => p.Time, p => p.CarOwnerID== ownerID);
            if (record == null)//没有充时长记录
            {
                return null;
            }
            return record.ExpireTime;
        }



    }
}
