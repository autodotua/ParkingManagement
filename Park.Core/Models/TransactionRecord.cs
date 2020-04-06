using System;
using System.ComponentModel.DataAnnotations;

namespace Park.Core.Models
{
    /// <summary>
    /// 交易记录
    /// </summary>
    public class TransactionRecord : IDbModel
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "车主")]
        [Required]
        public int CarOwnerID { get; set; }
        public CarOwner CarOwner { get; set; }
        [Display(Name = "交易时间")]
        [Required]
        public DateTime Time { get; set; }

        /// <summary>
        /// 正数代表用户付费
        /// 负数代表扣费
        /// </summary>
        [Display(Name = "交易值")]
        [Required]
        public double Value { get; set; }
        [Display(Name = "到期时间")]

        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "余额")]
        [Required]
        public double Balance { get; set; }

        [Display(Name = "类型")]
        [Required]
        public TransactionType Type { get; set; }
    }
    public enum TransactionType
    {
        [Display(Name = "停车")]
        /// <summary>
        /// 停车
        /// </summary>
        Park,
        /// <summary>
        /// 充值，针对预付费用户和月卡用户
        /// 月卡用户需要先充值，然后才可以充时长
        /// </summary>
        RechargeMoney,
        /// <summary>
        /// 充时长，针对月卡用户
        /// </summary>
        RechargeTime,
    }
}
