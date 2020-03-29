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
    public class ChangePasswordModel : BaseAdminModel
    {
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostChangePassword_btnSave_OnClickAsync(string tbxOldPassword, string tbxNewPassword, string tbxConfirmNewPassword)
        {
            int? id = GetIdentityID();

            // 检查当前密码是否正确
            string oldPass = tbxOldPassword.Trim();
            string newPass = tbxNewPassword.Trim();
            string confirmNewPass = tbxConfirmNewPassword.Trim();

            if (newPass != confirmNewPass)
            {
                UIHelper.TextBox("tbxConfirmNewPassword").MarkInvalid("确认密码和新密码不一致！");
            }
            else
            {
                User user = await DB.Users.Where(u => u.ID == id).AsNoTracking().FirstOrDefaultAsync();

                if (user != null)
                {
                    if (!PasswordUtil.ComparePasswords(user.Password, oldPass))
                    {
                        UIHelper.TextBox("tbxOldPassword").MarkInvalid("当前密码不正确！");
                    }
                    else
                    {
                        user.Password = PasswordUtil.CreateDbPassword(newPass);
                        await DB.SaveChangesAsync();

                        Alert.ShowInTop("修改密码成功！");
                    }
                }
            }

            return UIHelper.Result();
        }
    }
}