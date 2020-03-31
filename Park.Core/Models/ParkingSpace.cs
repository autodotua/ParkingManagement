using System.ComponentModel.DataAnnotations;

namespace Park.Core.Models
{
    public class ParkingSpace
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "类型")]
        [StringLength(50)]
        [Required]
        public string Class { get; set; } = "";
        [Display(Name = "停车区")]
        [Required]
        public ParkArea ParkArea { get; set; }

        [Display(Name = "是否已停车")]
        [Required]
        public bool HasCar { get; set; } = false;
    }


}
