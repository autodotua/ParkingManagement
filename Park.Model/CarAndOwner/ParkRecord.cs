using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Park.Models
{
    /// <summary>
    /// 停车记录
    /// </summary>
    public class ParkRecord : IDbModel
    {
        [Key]
        public int ID { get; set; }

        public ParkArea ParkArea { get; set; }
        [Display(Name = "停车区")]
        [Required]
        public int ParkAreaID { get; set; }

        [Display(Name = "车")]
        [Required]
        public Car Car { get; set; }
        [Display(Name = "车")]
        [Required]
        public int CarID { get; set; }
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
