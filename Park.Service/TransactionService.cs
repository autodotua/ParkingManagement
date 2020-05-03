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
    public static class TransactionService
    {
        /// <summary>
        /// 充时长
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
            double balance = await GetBalanceAsync(db, ownerID);
            if(balance<price*month)
            {
                return null;
            }
            DateTime? time = await GetExpireTimeAsync(db, ownerID);
            DateTime target;
            if(time.HasValue&&time.Value>DateTime.Now)
            {
                target = time.Value.AddMonths(month);
            }
            else
            {
                target = DateTime.Now.AddMonths(month);
            }
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
        public async static Task<TransactionRecord> RechargeMoneyAsync(ParkContext db,int ownerID,double amount)
        {
            Debug.Assert(amount > 0);
            double balance = await GetBalanceAsync(db, ownerID);
            TransactionRecord transactionRecord = new TransactionRecord()
            {
                Balance=balance+amount,
                CarOwnerID=ownerID,
                Time=DateTime.Now,
                Type=TransactionType.RechargeMoney,
                Value=amount
            };
            db.TransactionRecords.Add(transactionRecord);
            await db.SaveChangesAsync();
            return transactionRecord;
        }

        public async static Task<double> GetBalanceAsync(ParkContext db, int ownerID)
        {
            double? balance = (await db.TransactionRecords
                .LastOrDefaultRecordAsync(p => p.Time, p => p.CarOwnerID == ownerID))?.Balance;
            return balance ?? 0;
        }
        public async static Task<bool> IsMonthlyCardValidAsync(ParkContext db, int ownerID)
        {
            DateTime? expireTime = await GetExpireTimeAsync(db, ownerID);
            return expireTime.HasValue? expireTime >= DateTime.Now:false ;
        }

        private async static  Task<DateTime?> GetExpireTimeAsync(ParkContext db, int ownerID)
        {
            var record = await db.TransactionRecords
                .Where(p => p.Type == TransactionType.RechargeTime)//充时记录
                .LastOrDefaultRecordAsync(p => p.Time, p => p.CarOwnerID== ownerID);
            if (record == null)
            {//没有充时长记录
                return null;
            }
            return record.ExpireTime;
        }



    }
}
