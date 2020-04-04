using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Park.Core.Models
{
    public class Car:IDbModel
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "车牌")]
        [StringLength(50)]
        [Required]
        public string LicensePlate { get; set; }

        [Display(Name = "车主")]
        public CarOwner CarOwner { get; set; }
        [Display(Name = "车主")]
        public int? CarOwnerID { get; set; }

        public List<ParkRecord> ParkRecords { get; set; }
    }


}
