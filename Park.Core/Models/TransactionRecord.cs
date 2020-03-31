using System;
using System.ComponentModel.DataAnnotations;

namespace Park.Core.Models
{
    /// <summary>
    /// 交易记录
    /// </summary>
    public class TransactionRecord
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "车主")]
        [Required]
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
        /// <summary>
        /// 停车
        /// </summary>
        Park,
        /// <summary>
        /// 充值，针对预付费用户和月租用户
        /// </summary>
        Recharge,
    }
}
