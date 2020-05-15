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
    /// <summary>
    /// 为Park.Mobile提供交易相关信息
    /// </summary>
    public class TransactionController : ParkControllerBase
    {
        /// <summary>
        /// 获取主页信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取停车记录信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 用户充值操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
    /// <summary>
    /// 充值等操作请求体
    /// </summary>
    public class RechargeRequest : UserToken
    {
        /// <summary>
        /// 金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 充值的方式（支付宝、微信，该属性无用）
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 充值类型（money代表重置，time代表月租续期或开通）
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 充月租时代表充几个月
        /// </summary>
        public int Months { get; set; }
    }

}
