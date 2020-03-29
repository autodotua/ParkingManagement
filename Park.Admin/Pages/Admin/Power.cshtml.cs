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
    [CheckPower(Name = "CorePowerView")]
    public class PowerModel : BaseAdminModel
    {
        public IEnumerable<Power> Powers { get; set; }

        public PagingInfoViewModel PagingInfo { get; set; }

        public bool PowerCorePowerNew { get; set; }
        public bool PowerCorePowerEdit { get; set; }
        public bool PowerCorePowerDelete { get; set; }


        public async Task OnGetAsync()
        {
            Powers = await Power_LoadDataAsync();
        }

        private async Task<IEnumerable<Power>> Power_LoadDataAsync()
        {
            PowerCorePowerNew = CheckPower("CorePowerNew");
            PowerCorePowerEdit = CheckPower("CorePowerEdit");
            PowerCorePowerDelete = CheckPower("CorePowerDelete");

            var pagingInfo = new PagingInfoViewModel
            {
                SortField = "GroupName",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };
            PagingInfo = pagingInfo;

            return await Power_GetDataAsync(pagingInfo, String.Empty);
        }

        private async Task<IEnumerable<Power>> Power_GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage)
        {
            IQueryable<Power> q = DB.Powers;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(p => p.Name.Contains(searchText) || p.Title.Contains(searchText));
            }

            // 获取总记录数（在添加条件之后，排序和分页之前）
            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage<Power>(q, pagingInfo);

            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostPower_DoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
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
                if (!CheckPower("CorePowerDelete"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                int roleCount = await DB.Roles.Where(r => r.RolePowers.Any(p => p.PowerID == deletedRowID.Value)).CountAsync();
                if (roleCount > 0)
                {
                    Alert.ShowInTop("删除失败！需要先清空使用此权限的角色！");
                    return UIHelper.Result();
                }

                // 执行数据库操作
                var power = await DB.Powers.Where(m => m.ID == deletedRowID.Value).FirstOrDefaultAsync();
                DB.Powers.Remove(power);
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

            var powers = await Power_GetDataAsync(pagingInfo, ttbSearchMessage);
            // 1. 设置总项数
            grid1UI.RecordCount(pagingInfo.RecordCount);
            // 2. 设置每页显示项数
            if (actionType == "changeGridPageSize")
            {
                grid1UI.PageSize(ddlGridPageSize);
            }
            // 3.设置分页数据
            grid1UI.DataSource(powers, Grid1_fields);

            return UIHelper.Result();
        }
    }
}