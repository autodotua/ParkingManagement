using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Admin.Models;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CorePowerNew")]
    public class PowerNewModel : BaseAdminModel
    {
        [BindProperty]
        public Power Power { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostPowerNew_btnSaveClose_ClickAsync()
        {
            if (ModelState.IsValid)
            {
                DB.Powers.Add(Power);
                await DB.SaveChangesAsync();

                // 关闭本窗体（触发窗体的关闭事件）
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}