using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Mgt.Models;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Park.Mgt.Pages.Admin
{
    [CheckPower(Name = "CorePowerEdit")]
    public class PowerEditModel : BaseAdminModel
    {
        [BindProperty]
        public Power Power { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Power = await DB.Powers
                .Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();
				
            if (Power == null)
            {
                return Content("无效参数！");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostPowerEdit_btnSaveClose_ClickAsync()
        {
            if (ModelState.IsValid)
            {
                DB.Entry(Power).State = EntityState.Modified;
                await DB.SaveChangesAsync();

                // 关闭本窗体（触发窗体的关闭事件）
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}