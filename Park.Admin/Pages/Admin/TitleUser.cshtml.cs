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
    [CheckPower(Name = "CoreTitleUserView")]
    public class TitleUserModel : BaseAdminModel
    {
        public IEnumerable<Title> Titles { get; set; }
        public IEnumerable<User> Users { get; set; }

        public bool PowerCoreTitleUserNew { get; set; }
        public bool PowerCoreTitleUserDelete { get; set; }

        public string Grid1SelectedRowID { get; set; }
        public PagingInfoViewModel Grid1PagingInfo { get; set; }

        public PagingInfoViewModel Grid2PagingInfo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            PowerCoreTitleUserNew = CheckPower("CoreTitleUserNew");
            PowerCoreTitleUserDelete = CheckPower("CoreTitleUserDelete");

            // 表格1
            var grid1PagingInfo = new PagingInfoViewModel
            {
                SortField = "Name",
                SortDirection = "DESC"
            };
            Titles = await Sort<Title>(DB.Titles, grid1PagingInfo).ToListAsync();
            if (Titles.Count() == 0)
            {
                // 没有职称数据
                return Content("请先添加职称！");
            }
            var grid1SelectedRowID = Titles.First().ID;

            Grid1SelectedRowID = grid1SelectedRowID.ToString();
            Grid1PagingInfo = grid1PagingInfo;

            Users = await TitleUser_LoadDataAsync(grid1SelectedRowID);

            return Page();
        }

        private async Task<IEnumerable<User>> TitleUser_LoadDataAsync(int grid1SelectedRowID)
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

            return await TitleUser_GetDataAsync(grid2PagingInfo, grid1SelectedRowID, String.Empty);
        }

        private async Task<IEnumerable<User>> TitleUser_GetDataAsync(PagingInfoViewModel pagingInfo, int titleID, string ttbSearchMessage)
        {
            IQueryable<User> q = DB.Users;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.Name.Contains(searchText) || u.ChineseName.Contains(searchText) || u.EnglishName.Contains(searchText));
            }

            q = q.Where(u => u.Name != "admin");

            // 过滤选中职称下的所有用户
            q = q.Where(u => u.TitleUsers.Any(r => r.TitleID == titleID));

            // 在查询添加之后，排序和分页之前获取总记录数
            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage<User>(q, pagingInfo);

            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostTitleUser_Grid2_DoPostBackAsync(string[] Grid2_fields, int Grid2_pageIndex, string Grid2_sortField, string Grid2_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType, int selectedTitleId, int[] deletedUserIDs)
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
                if (!CheckPower("CoreTitleUserDelete"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                Title role = DB.Titles
                    .Include(r => r.TitleUsers)
                    .Where(r => r.ID == selectedTitleId)
                    .FirstOrDefault();

                //role.Users.Where(u => userIDs.Contains(u.ID)).ToList().ForEach(u => role.Users.Remove(u));
                foreach (int userID in deletedUserIDs)
                {
                    var user = role.TitleUsers.Where(u => u.UserID == userID).FirstOrDefault();
                    if (user != null)
                    {
                        role.TitleUsers.Remove(user);
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
            var titleUsers = await TitleUser_GetDataAsync(pagingInfo, selectedTitleId, ttbSearchMessage);
            // 1. 设置总项数
            grid2UI.RecordCount(pagingInfo.RecordCount);
            // 2. 设置每页显示项数
            if (actionType == "changeGridPageSize")
            {
                grid2UI.PageSize(ddlGridPageSize);
            }
            // 3.设置分页数据
            grid2UI.DataSource(titleUsers, Grid2_fields);


            return UIHelper.Result();
        }

        public async Task<IActionResult> OnPostTitleUser_Grid1_SortAsync(string[] Grid1_fields, string Grid1_sortField, string Grid1_sortDirection)
        {
            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection
            };

            grid1UI.DataSource(await Sort<Title>(DB.Titles, pagingInfo).ToListAsync(), Grid1_fields, clearSelection: false);

            return UIHelper.Result();
        }
    }
}