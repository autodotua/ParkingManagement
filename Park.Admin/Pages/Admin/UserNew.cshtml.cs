using Park.Admin.Models;


using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CoreUserNew")]
    public class UserNewModel : BaseAdminModel
    {
        [BindProperty]
        public User CurrentUser { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUserNew_btnSaveClose_ClickAsync(string hfSelectedDept, string hfSelectedRole, string hfSelectedTitle)
        {
            if (ModelState.IsValid)
            {
                var _user = await DB.Users.Where(u => u.Name == CurrentUser.Name).FirstOrDefaultAsync();
                if (_user != null)
                {
                    Alert.Show("用户 " + CurrentUser.Name + " 已经存在！");
                    return UIHelper.Result();
                }

                // 创建保存到数据库的密码
                CurrentUser.Password = PasswordUtil.CreateDbPassword(CurrentUser.Password.Trim());
                CurrentUser.CreateTime = DateTime.Now;

                // 添加所有部门
                if (!String.IsNullOrEmpty(hfSelectedDept))
                {
                    CurrentUser.DeptID = Convert.ToInt32(hfSelectedDept);
                }

                // 添加所有角色
                if (!String.IsNullOrEmpty(hfSelectedRole))
                {
                    int[] roleIDs = StringUtil.GetIntArrayFromString(hfSelectedRole);
                    AddEntities2<RoleUser>(roleIDs, CurrentUser.ID);
                }

                // 添加所有职称
                if (!String.IsNullOrEmpty(hfSelectedTitle))
                {
                    int[] titleIDs = StringUtil.GetIntArrayFromString(hfSelectedTitle);
                    AddEntities2<TitleUser>(titleIDs, CurrentUser.ID);
                }

                DB.Users.Add(CurrentUser);
                await DB.SaveChangesAsync();


                // 关闭本窗体（触发窗体的关闭事件）
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}