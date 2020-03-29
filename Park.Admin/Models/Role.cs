using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Park.Admin.Models
{
    public class Role : IKeyID
    {
        [Key]
        public int ID { get; set; }

        [Display(Name="名称")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "备注")]
        [StringLength(500)]
        public string Remark { get; set; }


        //public List<User> Users { get; set; }
        //public List<Power> Powers { get; set; }


        public List<RoleUser> RoleUsers { get; set; }

        public List<RolePower> RolePowers { get; set; }

    }
}