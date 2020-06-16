using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Park.Models;
using FineUICore;
using Park.Service;
using Microsoft.AspNetCore.Http;

namespace Park.Admin.Pages.Overview
{
    [CheckPower(Name = "ParkPower")]

    public class ParkSettingsModel : BaseModel
    {

        //public List<ParkArea> ParkAreas { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {

            ViewBag.Monthly = await Config.GetAsync(ParkDB, "MonthlyPrice", "120");
            ViewBag.Name = await Config.GetAsync(ParkDB, "Name", "停车场");
            return Page();
        }
        public async Task<IActionResult> OnPostSaveAsync(IFormCollection values)
        {
            await Config.SetAsync(ParkDB, "MonthlyPrice", values["txtMonthly"]);//设置月租120元/月
            await Config.SetAsync(ParkDB, "Name", values["txtName"]);
            ShowNotify("保存成功");
            return UIHelper.Result();
        }

    }
}