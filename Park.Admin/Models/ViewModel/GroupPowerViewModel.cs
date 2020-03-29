using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Park.Admin.Models
{
    public class GroupPowerViewModel
    {
        [Display(Name = "分组名称")]
        public string GroupName { get; set; }

        [Display(Name = "权限列表")]
        public JArray Powers { get; set; }

    }
}