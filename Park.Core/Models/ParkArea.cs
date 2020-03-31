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
        [Display(Name = "是否已停车")]
        [Required]
        public bool HasCar { get; set; } = false;
    }


}
