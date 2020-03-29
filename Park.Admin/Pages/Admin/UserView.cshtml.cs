using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Admin.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CoreUserView")]
    public class UserViewModel : BaseAdminModel
    {
        public User CurrentUser { get; set; }

        public string RoleText { get; set; }
        public string TitleText { get; set; }
        public string DeptText { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            CurrentUser = await DB.Users
                .Include(u => u.RoleUsers)
                .ThenInclude(ru => ru.Role)
                .Include(u => u.Dept)
                .Include(u => u.TitleUsers)
                .ThenInclude(tu => tu.Title)
                .Where(u => u.ID == id).AsNoTracking().FirstOrDefaultAsync();

            if (CurrentUser == null)
            {
                return Content("无效参数！");
            }

            if (CurrentUser.Name == "admin" && GetIdentityName() != "admin")
            {
                return Content("你无权编辑超级管理员！");
            }

            // 用户所属角色
            RoleText = String.Join(",", CurrentUser.RoleUsers.Select(ru => ru.Role.Name).ToArray());

            // 用户的职称列表
            TitleText = String.Join(",", CurrentUser.TitleUsers.Select(tu => tu.Title.Name).ToArray());


            // 用户所属的部门
            if (CurrentUser.DeptID != null)
            {
                DeptText = CurrentUser.Dept.Name;
            }

            return Page();
        }
    }
}