using System.ComponentModel.DataAnnotations;

namespace Park.Core.Models
{
    public class PriceStrategy
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 小时数上限
        /// </summary>
        [Required]
        public string StrategyJson { get; set; }
    }


}
