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
    [CheckPower(Name = "CoreTitleView")]
    public class TitleModel : BaseAdminModel
    {
        public IEnumerable<Title> Titles { get; set; }

        public PagingInfoViewModel PagingInfo { get; set; }

        public bool PowerCoreTitleNew { get; set; }
        public bool PowerCoreTitleEdit { get; set; }
        public bool PowerCoreTitleDelete { get; set; }

        public async Task OnGetAsync()
        {
            Titles = await Title_LoadDataAsync();
        }

        private async Task<IEnumerable<Title>> Title_LoadDataAsync()
        {
            PowerCoreTitleNew = CheckPower("CoreTitleNew");
            PowerCoreTitleEdit = CheckPower("CoreTitleEdit");
            PowerCoreTitleDelete = CheckPower("CoreTitleDelete");

            var pagingInfo = new PagingInfoViewModel
            {
                SortField = "Name",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };
            PagingInfo = pagingInfo;

            return await Title_GetDataAsync(pagingInfo, String.Empty);
        }

        private async Task<IEnumerable<Title>> Title_GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage)
        {
            IQueryable<Title> q = DB.Titles;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(p => p.Name.Contains(searchText));
            }

            // 获取总记录数（在添加条件之后，排序和分页之前）
            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage<Title>(q, pagingInfo);

            return q.ToList();
        }

        public async Task<IActionResult> OnPostTitle_DoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType, int? deletedRowID)
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
                if (!CheckPower("CoreTitleDelete"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                int userCount = await DB.Users.Where(u => u.TitleUsers.Any(r => r.TitleID == deletedRowID)).CountAsync();
                if (userCount > 0)
                {
                    Alert.ShowInTop("删除失败！需要先清空属于此职称的用户！");
                    return UIHelper.Result();
                }

                // 执行数据库操作
                var Title = await DB.Titles.Where(m => m.ID == deletedRowID.Value).FirstOrDefaultAsync();
                DB.Titles.Remove(Title);
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
            var titles = await Title_GetDataAsync(pagingInfo, ttbSearchMessage);
            // 1. 设置总项数
            grid1UI.RecordCount(pagingInfo.RecordCount);
            // 2. 设置每页显示项数
            if (actionType == "changeGridPageSize")
            {
                grid1UI.PageSize(ddlGridPageSize);
            }
            // 3.设置分页数据
            grid1UI.DataSource(titles, Grid1_fields);

            return UIHelper.Result();
        }
    }
}