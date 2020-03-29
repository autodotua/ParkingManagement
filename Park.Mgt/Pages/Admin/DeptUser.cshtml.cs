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
    [CheckPower(Name = "CoreDeptUserView")]
    public class DeptUserModel : BaseAdminModel
    {
        public IEnumerable<Dept> Depts { get; set; }
        public IEnumerable<User> Users { get; set; }

        public bool PowerCoreDeptUserNew { get; set; }
        public bool PowerCoreDeptUserDelete { get; set; }

        public string Grid1SelectedRowID { get; set; }

        public PagingInfoViewModel Grid2PagingInfo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            PowerCoreDeptUserNew = CheckPower("CoreDeptUserNew");
            PowerCoreDeptUserDelete = CheckPower("CoreDeptUserDelete");

            // 表格1
            Depts = DeptHelper.Depts;
            if (DeptHelper.Depts.Count == 0)
            {
                // 没有部门数据
                return Content("请先添加部门！");
            }
            var grid1SelectedRowID = DeptHelper.Depts[0].ID;
            Grid1SelectedRowID = grid1SelectedRowID.ToString();

            Users = await DeptUser_LoadDataAsync(grid1SelectedRowID);

            return Page();
        }

        private async Task<IEnumerable<User>> DeptUser_LoadDataAsync(int grid1SelectedRowID)
        {
            // 表格2
            var grid2PagingInfo = new PagingInfoViewModel
            {
                SortField = "Name",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };
            Grid2PagingInfo = grid2PagingInfo;
            return await DeptUser_GetDataAsync(grid2PagingInfo, grid1SelectedRowID, String.Empty);
        }

        private async Task<IEnumerable<User>> DeptUser_GetDataAsync(PagingInfoViewModel pagingInfo, int deptID, string ttbSearchMessage)
        {
            IQueryable<User> q = DB.Users;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            }

            q = q.Where(u => u.Name != "admin");

            // 过滤选中部门下的所有用户
            q = q.Where(u => u.Dept.ID == deptID);

            // 获取总记录数（在添加条件之后，排序和分页之前）
            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage<User>(q, pagingInfo);

            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeptUser_Grid2_DoPostBackAsync(string[] Grid2_fields, int Grid2_pageIndex, string Grid2_sortField, string Grid2_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType, int selectedDeptId, int[] deletedUserIDs)
        {
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
                if (!CheckPower("CoreDeptUserDelete"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                Dept role = await DB.Depts
                    .Include(r => r.Users)
                    .Where(r => r.ID == selectedDeptId)
                    .FirstOrDefaultAsync();

                foreach (int userID in deletedUserIDs)
                {
                    User user = role.Users.Where(u => u.ID == userID).FirstOrDefault();
                    if (user != null)
                    {
                        role.Users.Remove(user);
                    }
                }

                await DB.SaveChangesAsync();
            }

            var grid2UI = UIHelper.Grid("Grid2");
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid2_sortField,
                SortDirection = Grid2_sortDirection,
                PageIndex = Grid2_pageIndex,
                PageSize = ddlGridPageSize
            };
            var deptUsers = await DeptUser_GetDataAsync(pagingInfo, selectedDeptId, ttbSearchMessage);
            // 1. 设置总项数
            grid2UI.RecordCount(pagingInfo.RecordCount);
            // 2. 设置每页显示项数
            if (actionType == "changeGridPageSize")
            {
                grid2UI.PageSize(ddlGridPageSize);
            }
            // 3.设置分页数据
            grid2UI.DataSource(deptUsers, Grid2_fields);


            return UIHelper.Result();
        }
    }
}