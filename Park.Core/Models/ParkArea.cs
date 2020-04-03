using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Park.Core.Models
{
    /// <summary>
    /// 停车区
    /// </summary>
    public class ParkArea
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "名称")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "价格策略")]
        public int? PriceStrategyID { get; set; }
        public PriceStrategy PriceStrategy { get; set; }
        public List<ParkingSpace> ParkingSpaces { get; set; }
        public List<Aisle> Aisles { get; set; }

        public double Length { get; set; }
        public double Width { get; set; }
    }


}
