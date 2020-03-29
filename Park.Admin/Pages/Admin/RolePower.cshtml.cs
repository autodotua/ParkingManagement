using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Park.Admin.Models;

using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CoreRolePowerView")]
    public class RolePowerModel : BaseAdminModel
    {
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<GroupPowerViewModel> GroupPowers { get; set; }

        public PagingInfoViewModel Grid1PagingInfo { get; set; }
        public PagingInfoViewModel Grid2PagingInfo { get; set; }

        public bool PowerCoreRolePowerEdit { get; set; }

        public string Grid1SelectedRowID { get; set; }

        public string RolePowerIds { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            PowerCoreRolePowerEdit = CheckPower("CoreRolePowerEdit");

            // 表格1
            var grid1PagingInfo = new PagingInfoViewModel
            {
                SortField = "Name",
                SortDirection = "DESC"
            };

            Roles = await Sort<Role>(DB.Roles, grid1PagingInfo).ToListAsync();
            if (Roles.Count() == 0)
            {
                // 没有角色数据
                return Content("请先添加角色！");
            }
            var grid1SelectedRowID = Roles.First().ID;

            Grid1SelectedRowID = grid1SelectedRowID.ToString();
            Grid1PagingInfo = grid1PagingInfo;

            GroupPowers = await RolePower_LoadDataAsync(grid1SelectedRowID);

            return Page();
        }

        private async Task<IEnumerable<GroupPowerViewModel>> RolePower_LoadDataAsync(int grid1SelectedRowID)
        {
            // 当前选中角色拥有的权限列表
            RolePowerIds = await RolePower_GetRolePowerIdsAsync(grid1SelectedRowID);

            // 表格2
            var grid2PagingInfo = new PagingInfoViewModel
            {
                SortField = "GroupName",
                SortDirection = "DESC",
            };
            Grid2PagingInfo = grid2PagingInfo;

            return await RolePower_GetDataAsync(grid2PagingInfo);
        }

        private async Task<IEnumerable<GroupPowerViewModel>> RolePower_GetDataAsync(PagingInfoViewModel pagingInfo)
        {
            // Client side GroupBy is not supported.
            // https://stackoverflow.com/questions/58138556/client-side-groupby-is-not-supported
            // https://stackoverflow.com/questions/60432078/asp-net-core-web-api-client-side-groupby-is-not-supported

            //var q = DB.Powers.GroupBy(p => p.GroupName);
            //if (pagingInfo.SortField == "GroupName")
            //{
            //    if (pagingInfo.SortDirection == "ASC")
            //    {
            //        q = q.OrderBy(g => g.Key);
            //    }
            //    else
            //    {
            //        q = q.OrderByDescending(g => g.Key);
            //    }
            //}
            //var powers = await q.ToListAsync();

            var powers = (await DB.Powers.ToListAsync()).GroupBy(p => p.GroupName);
            if (pagingInfo.SortField == "GroupName")
            {
                if (pagingInfo.SortDirection == "ASC")
                {
                    powers = powers.OrderBy(g => g.Key);
                }
                else
                {
                    powers = powers.OrderByDescending(g => g.Key);
                }
            }


            List<GroupPowerViewModel> groupPowers = new List<GroupPowerViewModel>();
            foreach (var power in powers)
            {
                var groupPower = new GroupPowerViewModel();
                groupPower.GroupName = power.Key;

                JArray ja = new JArray();
                foreach (var powerItem in power.ToList())
                {
                    JObject jo = new JObject();
                    jo.Add("id", powerItem.ID);
                    jo.Add("name", powerItem.Name);
                    jo.Add("title", powerItem.Title);
                    ja.Add(jo);
                }
                groupPower.Powers = ja;

                groupPowers.Add(groupPower);
            }

            return groupPowers;
        }

        private async Task<string> RolePower_GetRolePowerIdsAsync(int grid1SelectedRowID)
        {
            // 当前选中角色拥有的权限列表
            Role role = await DB.Roles
                .Include(r => r.RolePowers)
                .Where(r => r.ID == grid1SelectedRowID).FirstOrDefaultAsync();

            return new JArray(role.RolePowers.Select(p => p.PowerID)).ToString(Newtonsoft.Json.Formatting.None);
        }

        public async Task<IActionResult> OnPostRolePower_Grid1_Sort(string[] Grid1_fields, string Grid1_sortField, string Grid1_sortDirection)
        {
            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection
            };


            grid1UI.DataSource(await Sort<Role>(DB.Roles, pagingInfo).ToListAsync(), Grid1_fields, clearSelection: false);

            return UIHelper.Result();
        }

        public async Task<IActionResult> OnPostRolePower_Grid2_DoPostBackAsync(string[] Grid2_fields, string Grid2_sortField, string Grid2_sortDirection,
            string actionType, int selectedRoleID, int[] selectedPowerIDs)
        {
            // 保存角色权限时，不需要重新加载表格数据
            if (actionType == "saveall")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreRolePowerEdit"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }


                // 当前角色新的权限列表
                Role role = await DB.Roles.Include(r => r.RolePowers).Where(r => r.ID == selectedRoleID).FirstOrDefaultAsync();

                ReplaceEntities2<RolePower>(role.RolePowers, selectedRoleID, selectedPowerIDs);

                await DB.SaveChangesAsync();

                Alert.ShowInTop("当前角色的权限更新成功！");
            }
            else
            {
                var grid2UI = UIHelper.Grid("Grid2");
                var pagingInfo = new PagingInfoViewModel
                {
                    SortField = Grid2_sortField,
                    SortDirection = Grid2_sortDirection
                };
                grid2UI.DataSource(await RolePower_GetDataAsync(pagingInfo), Grid2_fields);

                // 更新当前角色的权限
                FineUICore.PageContext.RegisterStartupScript("updateRolePowers(" + await RolePower_GetRolePowerIdsAsync(selectedRoleID) + ");");
            }

            return UIHelper.Result();
        }
    }
}