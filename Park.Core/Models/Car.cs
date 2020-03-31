using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Park.Core.Models
{
    public class Car
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "车牌")]
        [StringLength(50)]
        [Required]
        public string LicensePlate { get; set; }

        [Display(Name = "车主")]
        [Required]
        public CarOwner CarOwner { get; set; }
       
    }


}
