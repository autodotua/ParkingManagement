using System.ComponentModel.DataAnnotations;

namespace Park.Core.Models
{
    public class CarOwner
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "用户名")]
        [StringLength(50)]
        [Required]
        public string Username { get; set; }

        [Display(Name = "密码")]
        [StringLength(50)]
        [Required]
        public string Password { get; set; }
        [Display(Name = "付费类型")]
        [Required]
        public PaymentType Type { get; set; } = PaymentType.General;
        [Display(Name = "是否启用")]
        [Required]
        public bool Enabled { get; set; }
    }
    public enum PaymentType
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        General,
        /// <summary>
        /// 预付费
        /// </summary>
        Prepaid,
        /// <summary>
        /// 月付费
        /// </summary>
        Monthly,
        /// <summary>
        /// 免费
        /// </summary>
        Free
    }

}
