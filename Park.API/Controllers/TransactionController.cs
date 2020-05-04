using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Park.Models;
using Park.Service;


namespace Park.API.Controllers
{
    public class TransactionController : ParkControllerBase
    {

        [HttpPost]
        [Route("Index")]
        public async Task<ResponseData<TransactionRecord>> IndexAsync([FromBody] UserToken request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<TransactionRecord>() { Succeed = false, Message = "用户验证失败" };
            }
            TransactionRecord record = await db.TransactionRecords
                .LastOrDefaultRecordAsync(p => p.Time, p => p.CarOwnerID == request.UserID);
            if (record == null)
            {
                record = new TransactionRecord();
            }
            return new ResponseData<TransactionRecord>(record);

        }
        [HttpPost]
        [Route("Records")]
        public async Task<ResponseData<List<TransactionRecord>>> RecordsAsync([FromBody] RechargeRequest request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<List<TransactionRecord>>() { Succeed = false, Message = "用户验证失败" };
            }
            DateTime now = DateTime.Now;
            var records =await db.TransactionRecords
                .Where(p => p.CarOwnerID == request.UserID)
                .OrderByDescending(p=>p.Time).Take(120).ToListAsync();//仅提取最后120条数据
            return new ResponseData<List<TransactionRecord>>(records);
        }
        [HttpPost]
        [Route("Recharge")]
        public async Task<ResponseData<TransactionRecord>> RechargeAsync([FromBody] RechargeRequest request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<TransactionRecord>() { Succeed = false, Message = "用户验证失败" };
            }
            //此处需要进行验证

            TransactionRecord record = new TransactionRecord()
            {
                CarOwnerID = request.UserID,
                Time = DateTime.Now
            };
            switch (request.Type)
            {
                case "money":
                    {
                        var result = await TransactionService.RechargeMoneyAsync(db, request.UserID, request.Amount);
                        return new ResponseData<TransactionRecord>(result);
                    }
                case "time":
                    {
                        var result = await TransactionService.RechargeTimeAsync(db, request.UserID, request.Months);
                        if (result == null)
                        {
                            return new ResponseData<TransactionRecord>() { Message = "充值失败，余额不足", Succeed = false };
                        }
                        return new ResponseData<TransactionRecord>(result);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

    }

    public class RechargeRequest : UserToken
    {
        public double Amount { get; set; }
        public string Method { get; set; }
        public string Type { get; set; }//money/time
        public int Months { get; set; }
    }

}
