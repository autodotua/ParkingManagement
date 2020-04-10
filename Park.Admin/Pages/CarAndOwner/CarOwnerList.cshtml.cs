using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Park.Admin.Models;
using Park.Models;
using Park.Service;

namespace Park.Admin.Pages.CarAndOwner
{
    [CheckPower(Name = "CarAndOwnerPower")]

    public class ExtendCarOwner : CarOwner
    {
        public ExtendCarOwner()
        {
        }
        [Display(Name = "余额")]
        public double Balance { get; set; }

        public async Task<ExtendCarOwner> Apply(CarOwner owner, ParkContext db)
        {
            ID = owner.ID;
            Enabled = owner.Enabled;
            Password = owner.Password;
            Username = owner.Username;
            IsFree = owner.IsFree;
            Cars = owner.Cars;
            TransactionRecords = owner.TransactionRecords;
            Balance = await TransactionService.GetBalanceAsync(db, owner);
            return this;
        }
    }
    public class CarOwnerListModel : GridBaseModel<CarOwner>
    {
        public IEnumerable<ExtendCarOwner> CarOwners { get; set; }


        public override DbSet<CarOwner> DbSets => ParkDB.CarOwners;

        public async Task OnGetAsync()
        {
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = "Username",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };

            PagingInfo = pagingInfo;

            CarOwners = (await GetDataAsync(pagingInfo, string.Empty)).Cast<ExtendCarOwner>();
        }

    

        protected override async Task<IEnumerable<CarOwner>> GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage)
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

        

            // 获取总记录数（在添加条件之后，排序和分页之前）
            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage(q, pagingInfo);

            return (await q.Include(p=>p.TransactionRecords).Include(p=>p.Cars).ToListAsync())
                .Select(p => new ExtendCarOwner().Apply(p, ParkDB).Result);
        }

        public async Task<IActionResult> OnPostCarOwnerList_DoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
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

                ParkDB.CarOwners.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => u.Enabled = true);
                await DB.SaveChangesAsync();
            }
            else if (actionType == "pswd")
            {
                if (!CheckPower("CoreUserEdit"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                foreach (var owner in ParkDB.CarOwners.Where(u => ids.Contains(u.ID)).ToList())
                {
                    await CarOwnerService.SetPasswordAsync(ParkDB, owner, "123456");
                }
                ShowNotify("已重设密码为123456");
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

            return await LoadGrid(Grid1_fields, Grid1_pageIndex, Grid1_sortField, Grid1_sortDirection, ttbSearchMessage, ddlGridPageSize, actionType);
        }

        protected async override Task OtherPostBackAsync(string actionType,List<int> ids)
        {
            if (actionType == "pswd")
            {
                foreach (var owner in ParkDB.CarOwners.Where(u => ids.Contains(u.ID)).ToList())
                {
                    await CarOwnerService.SetPasswordAsync(ParkDB, owner, "123456");
                }
                ShowNotify("已重设密码为123456");
                await DB.SaveChangesAsync();
            }
        }

        //private async Task<IActionResult> LoadGrid(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection, string ttbSearchMessage, int ddlGridPageSize, string actionType)
        //{
        //    var grid1UI = UIHelper.Grid("Grid1");
        //    var pagingInfo = new PagingInfoViewModel
        //    {
        //        SortField = Grid1_sortField,
        //        SortDirection = Grid1_sortDirection,
        //        PageIndex = Grid1_pageIndex,
        //        PageSize = ddlGridPageSize
        //    };

        //    var users = await GetDataAsync(pagingInfo, ttbSearchMessage);
        //    // 1. 设置总项数
        //    grid1UI.RecordCount(pagingInfo.RecordCount);
        //    // 2. 设置每页显示项数
        //    if (actionType == "changeGridPageSize")
        //    {
        //        grid1UI.PageSize(ddlGridPageSize);
        //    }
        //    // 3.设置分页数据
        //    grid1UI.DataSource(users, Grid1_fields);

        //    return UIHelper.Result();
        //}

        public async Task<IActionResult> OnPostSaveDataAsync(string[] Grid1_fields, JArray Grid1_modifiedData, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, int ddlGridPageSize, string actionType)
        {

            foreach (JObject modifiedRow in Grid1_modifiedData)
            {
                string status = modifiedRow.Value<string>("status");
                int rowId = Convert.ToInt32(modifiedRow.Value<string>("id"));

                if (status == "modified")
                {
                    var owner = ParkDB.CarOwners.Find(rowId);
                    owner.Enabled = modifiedRow["values"]["Enabled"].Value<bool>();
                    ParkDB.Entry(owner).State = EntityState.Modified;
                }
            }
            await ParkDB.SaveChangesAsync();
            ShowNotify("数据保存成功！");

            return await LoadGrid(Grid1_fields, Grid1_pageIndex, Grid1_sortField, Grid1_sortDirection, ttbSearchMessage, ddlGridPageSize, actionType);
        }

    }
}