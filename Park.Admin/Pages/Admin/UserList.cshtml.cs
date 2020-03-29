using Park.Admin.Models;


using FineUICore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CoreUserView")]
    public class UserListModel : BaseAdminModel
    {
        public IEnumerable<User> Users { get; set; }
        public PagingInfoViewModel PagingInfo { get; set; }

        public bool PowerCoreUserNew { get; set; }
        public bool PowerCoreUserEdit { get; set; }
        public bool PowerCoreUserDelete { get; set; }
        public bool PowerCoreUserChangePassword { get; set; }

        public async Task OnGetAsync()
        {
            await UserList_LoadDataAsync();
        }

        private async Task UserList_LoadDataAsync()
        {
            PowerCoreUserNew = CheckPower("CoreUserNew");
            PowerCoreUserEdit = CheckPower("CoreUserEdit");
            PowerCoreUserDelete = CheckPower("CoreUserDelete");
            PowerCoreUserChangePassword = CheckPower("CoreUserChangePassword");

            var pagingInfo = new PagingInfoViewModel
            {
                SortField = "Name",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };

            PagingInfo = pagingInfo;

            Users = await UserList_GetDataAsync(pagingInfo, String.Empty, "all");
        }

        private async Task<IEnumerable<User>> UserList_GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage, string rblEnableStatus)
        {
            IQueryable<User> q = DB.Users;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            }

            if (GetIdentityName() != "admin")
            {
                q = q.Where(u => u.Name != "admin");
            }

            // 过滤启用状态
            if (rblEnableStatus != "all")
            {
                q = q.Where(u => u.Enabled == (rblEnableStatus == "enabled" ? true : false));
            }


            // 获取总记录数（在添加条件之后，排序和分页之前）
            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage<User>(q, pagingInfo);

            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostUserList_DoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, string rblEnableStatus, int ddlGridPageSize, string actionType, int[] deletedRowIDs)
        {
            List<int> ids = new List<int>();
            if (deletedRowIDs != null)
            {
                ids.AddRange(deletedRowIDs);
            }

            var ttbSearchMessageUI = UIHelper.TwinTriggerBox("ttbSearchMessage");
            if (actionType == "trigger1")
            {
                ttbSearchMessageUI.Text(String.Empty);
                ttbSearchMessageUI.ShowTrigger1(false);

                // 清空传入的搜索值
                ttbSearchMessage = String.Empty;
            }
            else if (actionType == "trigger2")
            {
                ttbSearchMessageUI.ShowTrigger1(true);
            }
            else if (actionType == "delete")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreUserDelete"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                DB.Users.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => DB.Users.Remove(u));
                await DB.SaveChangesAsync();
            }
            else if (actionType == "enable")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreUserEdit"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                DB.Users.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => u.Enabled = true);
                await DB.SaveChangesAsync();
            }
            else if (actionType == "disable")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreUserEdit"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                DB.Users.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => u.Enabled = false);
                await DB.SaveChangesAsync();
            }


            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };

            var users = await UserList_GetDataAsync(pagingInfo, ttbSearchMessage, rblEnableStatus);
            // 1. 设置总项数
            grid1UI.RecordCount(pagingInfo.RecordCount);
            // 2. 设置每页显示项数
            if (actionType == "changeGridPageSize")
            {
                grid1UI.PageSize(ddlGridPageSize);
            }
            // 3.设置分页数据
            grid1UI.DataSource(users, Grid1_fields);

            return UIHelper.Result();
        }
    }
}