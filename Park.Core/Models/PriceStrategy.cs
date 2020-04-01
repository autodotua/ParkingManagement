using System.ComponentModel.DataAnnotations;

namespace Park.Core.Models
{
    public class PriceStrategy
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string StrategyJson { get; set; }
        public double MonthlyPrice { get; set; }
    }


}
