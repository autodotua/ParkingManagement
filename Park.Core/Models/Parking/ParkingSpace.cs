using System.ComponentModel.DataAnnotations;

namespace Park.Core.Models
{
    public class ParkingSpace: IParkObject
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "类型")]
        [StringLength(50)]
        [Required]
        public string Class { get; set; } = "";
        [Display(Name = "停车区")]
        [Required]
        public int ParkAreaID { get; set; }
        public ParkArea ParkArea { get; set; }

        [Display(Name = "是否已停车")]
        [Required]
        public bool HasCar { get; set; } = false;

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RotateAngle { get; set; }
        [Display(Name = "传感器的token")]
        public string SensorToken { get; set; }
    }


}
