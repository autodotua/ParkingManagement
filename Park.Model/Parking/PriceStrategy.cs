using System.ComponentModel.DataAnnotations;

namespace Park.Models
{
    public class PriceStrategy:IDbModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string StrategyJson { get; set; }
        //public double MonthlyPrice { get; set; }
    }


}
