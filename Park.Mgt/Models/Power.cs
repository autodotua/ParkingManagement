using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Park.Mgt.Models
{
    public class Power : IKeyID
    {
        [Key]
        public int ID { get; set; }

        [Display(Name="名称")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "分组名称")]
        [StringLength(50)]
        [Required]
        public string GroupName { get; set; }

        [Display(Name = "标题")]
        [StringLength(200)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "备注")]
        [StringLength(500)]
        public string Remark { get; set; }



        //public List<Role> Roles { get; set; }

        public List<RolePower> RolePowers { get; set; }

    }
}