using System;
using System.ComponentModel.DataAnnotations;

namespace Park.Core.Models
{
    /// <summary>
    /// 停车记录
    /// </summary>
    public class ParkRecord
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "车主")]
        [Required]
        public Car Car { get; set; }
        [Display(Name = "进场时间")]
        [Required]
        public DateTime EnterTime { get; set; }
        [Display(Name = "离场时间")]
        [Required]
        public DateTime LeaveTime { get; set; }
        [Display(Name = "交易记录")]
        public TransactionRecord TransactionRecord { get; set; }
    }
}
