using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Park.Mgt.Models;

using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Park.Mgt.Pages.Admin
{
    [CheckPower(Name = "CoreUserEdit")]
    public class UserEditModel : BaseAdminModel
    {
        [BindProperty]
        public User CurrentUser { get; set; }

        public string SelectedRoleNames { get; set; }
        public string SelectedRoleIDs { get; set; }

        public string SelectedTitleNames { get; set; }
        public string SelectedTitleIDs { get; set; }

        public string SelectedDeptName { get; set; }
        public string SelectedDeptID { get; set; }




        public async Task<IActionResult> OnGetAsync(int id)
        {
            CurrentUser = await DB.Users
                .Include(u => u.Dept)
                .Include(u => u.RoleUsers)
                .ThenInclude(ru => ru.Role)
                .Include(u => u.TitleUsers)
                .ThenInclude(tu => tu.Title)
                .Where(m => m.ID == id).AsNoTracking().FirstOrDefaultAsync();
            if (CurrentUser == null)
            {
                return Content("无效参数！");
            }

            if (CurrentUser.Name == "admin" && GetIdentityName() != "admin")
            {
                return Content("你无权编辑超级管理员！");
            }

            // 用户所属角色
            SelectedRoleNames = String.Join(",", CurrentUser.RoleUsers.Select(ru => ru.Role.Name).ToArray());
            SelectedRoleIDs = String.Join(",", CurrentUser.RoleUsers.Select(ru => ru.RoleID).ToArray());

            // 用户拥有职称
            SelectedTitleNames = String.Join(",", CurrentUser.TitleUsers.Select(tu => tu.Title.Name).ToArray());
            SelectedTitleIDs = String.Join(",", CurrentUser.TitleUsers.Select(tu => tu.TitleID).ToArray());


            // 用户所属部门
            if (CurrentUser.Dept != null)
            {
                SelectedDeptName = CurrentUser.Dept.Name;
                SelectedDeptID = CurrentUser.Dept.ID.ToString();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUserEdit_btnSaveClose_ClickAsync(string hfSelectedDept, string hfSelectedRole, string hfSelectedTitle)
        {
            // 不对 Name 和 Password 进行模型验证
            ModelState.Remove("Name");
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                // 更新部分字段（先从数据库检索用户，再覆盖用户输入值，注意没有更新Name，Password，CreateTime等字段）
                var _user = await DB.Users
                    .Include(u => u.Dept)
                    .Include(u => u.RoleUsers)
                    .Include(u => u.TitleUsers)
                    .Where(m => m.ID == CurrentUser.ID).FirstOrDefaultAsync();


                _user.ChineseName = CurrentUser.ChineseName;
                _user.Gender = CurrentUser.Gender;
                _user.Enabled = CurrentUser.Enabled;
                _user.Email = CurrentUser.Email;
                _user.CompanyEmail = CurrentUser.CompanyEmail;
                _user.OfficePhone = CurrentUser.OfficePhone;
                _user.OfficePhoneExt = CurrentUser.OfficePhoneExt;
                _user.HomePhone = CurrentUser.HomePhone;
                _user.CellPhone = CurrentUser.CellPhone;
                _user.Remark = CurrentUser.Remark;


                int[] roleIDs = StringUtil.GetIntArrayFromString(hfSelectedRole);
                ReplaceEntities2<RoleUser>(_user.RoleUsers, roleIDs, _user.ID);

                int[] titleIDs = StringUtil.GetIntArrayFromString(hfSelectedTitle);
                ReplaceEntities2<TitleUser>(_user.TitleUsers, titleIDs, _user.ID);

                if (String.IsNullOrEmpty(hfSelectedDept))
                {
                    _user.DeptID = null;
                }
                else
                {
                    _user.DeptID = Convert.ToInt32(hfSelectedDept);
                }

                await DB.SaveChangesAsync();

                // 关闭本窗体（触发窗体的关闭事件）
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}