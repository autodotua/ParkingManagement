using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Park.Admin.Models
{
    public class Config : IKeyID
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "键")]
        [StringLength(50)]
        [Required]
        public string ConfigKey { get; set; }

        [Display(Name = "值")]
        [StringLength(4000)]
        [Required]
        public string ConfigValue { get; set; }

        [Display(Name = "备注")]
        [StringLength(500)]
        public string Remark { get; set; }
    }
}