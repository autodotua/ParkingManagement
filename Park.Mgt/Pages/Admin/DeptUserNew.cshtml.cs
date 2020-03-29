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
    [CheckPower(Name = "CoreDeptUserNew")]
    public class DeptUserNewModel : BaseAdminModel
    {
        public Dept Dept { get; set; }

        public IEnumerable<User> Users { get; set; }

        public PagingInfoViewModel PagingInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int deptID)
        {
            Dept = await DB.Depts
                .Where(d => d.ID == deptID).AsNoTracking().FirstOrDefaultAsync();

            if (Dept == null)
            {
                return Content("无效参数！");
            }

            Users = await DeptUserNew_LoadDataAsync(deptID);

            return Page();
        }

        private async Task<IEnumerable<User>> DeptUserNew_LoadDataAsync(int deptID)
        {
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = "Name",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };
            PagingInfo = pagingInfo;

            return await DeptUserNew_GetDataAsync(pagingInfo, deptID, String.Empty);
        }

        private async Task<IEnumerable<User>> DeptUserNew_GetDataAsync(PagingInfoViewModel pagingInfo, int deptID, string ttbSearchMessage)
        {
            IQueryable<User> q = DB.Users;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            }

            q = q.Where(u => u.Name != "admin");

            // 排除所有已经属于某个部门的用户
            q = q.Where(u => u.Dept == null);

            // 获取总记录数（在添加条件之后，排序和分页之前）
            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage<User>(q, pagingInfo);

            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeptUserNew_DoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType, int deptID)
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


            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };


            //grid1UI.DataSource(await DeptUserNew_GetDataAsync(pagingInfo, deptID, ttbSearchMessage), Grid1_fields, clearSelection: false);
            //grid1UI.RecordCount(pagingInfo.RecordCount);

            var deptUsers = await DeptUserNew_GetDataAsync(pagingInfo, deptID, ttbSearchMessage);
            // 1. 设置总项数
            grid1UI.RecordCount(pagingInfo.RecordCount);
            // 2. 设置每页显示项数
            if (actionType == "changeGridPageSize")
            {
                grid1UI.PageSize(ddlGridPageSize);
            }
            // 3.设置分页数据
            grid1UI.DataSource(deptUsers, Grid1_fields, clearSelection: false);

            return UIHelper.Result();
        }

        public async Task<IActionResult> OnPostDeptUserNew_btnSaveClose_ClickAsync(int deptID, int[] selectedRowIDs)
        {
            var users = await DB.Users
                 .Where(u => selectedRowIDs.Contains(u.ID))
                 .ToListAsync();

            users.ForEach(u => u.DeptID = deptID);

            await DB.SaveChangesAsync();

            // 关闭本窗体（触发窗体的关闭事件）
            ActiveWindow.HidePostBack();

            return UIHelper.Result();
        }
    }
}