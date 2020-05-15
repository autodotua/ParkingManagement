using System.ComponentModel.DataAnnotations;

namespace Park.Models
{
    public class PriceStrategy:IDbModel
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 定价策略
        /// </summary>
        [Required]
        public string StrategyJson { get; set; }
        //public double MonthlyPrice { get; set; }

        /*
         {
         "type": "stepHourBase",
         "prices": [{
                 "upper": 5,
                 "price": 2

             },
             {
                 "upper": 12,
                 "price": 1
             },
             {
                 "upper": -1,
                 "price": 0.5
             }
         ]
     }*/
    }


}
