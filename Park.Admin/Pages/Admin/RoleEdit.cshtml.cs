using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Admin.Models;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CoreRoleEdit")]
    public class RoleEditModel : BaseAdminModel
    {
        [BindProperty]
        public Role Role { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Role = await DB.Roles
                .Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();


            if (Role == null)
            {
                return Content("无效参数！");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRoleEdit_btnSaveClose_ClickAsync()
        {
            if (ModelState.IsValid)
            {
                DB.Entry(Role).State = EntityState.Modified;
                await DB.SaveChangesAsync();

                // 关闭本窗体（触发窗体的关闭事件）
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}