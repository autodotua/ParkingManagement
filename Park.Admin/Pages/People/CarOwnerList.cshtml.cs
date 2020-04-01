using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Park.Admin.Models;
using Park.Core.Models;

namespace Park.Admin.Pages.People
{
    public class ExtendCarOwners : CarOwner
    {
        public ExtendCarOwners()
        {
        }
        [Display(Name ="余额")]
        public double Balance { get; set; }

        public ExtendCarOwners(CarOwner owner,ParkContext db)
        {
            ID = owner.ID;
            Enabled = owner.Enabled;
            Password = owner.Password;
            Username = owner.Username;
            if (db.TransactionRecords.Any(p => p.CarOwner.ID == ID))
            {
                Balance = db.TransactionRecords
                    .Where(p=> p.CarOwner.ID == ID)
                    .Last().Balance;
            }
            else
            {
                Balance = 0;
            }
        }
    }
    public class CarOwnerListModel : BaseModel
    {
        public IEnumerable<ExtendCarOwners> CarOwners { get; set; }

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
                SortField = "Username",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };

            PagingInfo = pagingInfo;

            CarOwners = await UserList_GetDataAsync(pagingInfo, string.Empty, "all");
        }

        private async Task<IEnumerable<ExtendCarOwners>> UserList_GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage, string rblEnableStatus)
        {
            IQueryable<CarOwner> q = ParkDB.CarOwners;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.Username.Contains(searchText));
            }

            if (GetIdentityName() != "admin")
            {
                q = q.Where(u => u.Username != "admin");
            }

            // 过滤启用状态
            if (rblEnableStatus != "all")
            {
                q = q.Where(u => u.Enabled == (rblEnableStatus == "enabled" ? true : false));
            }


            // 获取总记录数（在添加条件之后，排序和分页之前）
            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage(q, pagingInfo);

            return (await q.ToListAsync()).Select(p => new ExtendCarOwners(p,ParkDB));
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

                ParkDB.CarOwners.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => ParkDB.CarOwners.Remove(u));
                await ParkDB.SaveChangesAsync();
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