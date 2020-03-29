using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Admin.Models;

using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CoreUserChangePassword")]
    public class UserChangePasswordModel : BaseAdminModel
    {
        public User CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            CurrentUser = await DB.Users.Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();

            if (CurrentUser == null)
            {
                return Content("无效参数！");
            }

            if (CurrentUser.Name == "admin" && GetIdentityName() != "admin")
            {
                return Content("你无权编辑超级管理员！");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUserChangePassword_btnSaveClose_ClickAsync(int hfUserID, string tbxPassword)
        {
            var item = await DB.Users.FindAsync(hfUserID);

            item.Password = PasswordUtil.CreateDbPassword(tbxPassword.Trim());
            await DB.SaveChangesAsync();

            // 关闭本窗体（触发窗体的关闭事件）
            ActiveWindow.HidePostBack();

            return UIHelper.Result();
        }
    }
}