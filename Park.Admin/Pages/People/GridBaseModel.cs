using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Park.Admin.Models;
using Park.Core.Models;
using Park.Core.Service;

namespace Park.Admin.Pages.People
{
    public abstract class GridBaseModel<T> : BaseModel where T : class, IDbModel
    {
        public PagingInfoViewModel PagingInfo { get; set; }
        public abstract DbSet<T> DbSets { get; }
        public async Task<IActionResult> OnPostDoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
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
                DbSets.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => DbSets.Remove(u));
                await ParkDB.SaveChangesAsync();
            }
            //else if (actionType == "enable")
            //{
            //    DbSets.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => u.Enabled = true);
            //    await DB.SaveChangesAsync();
            //}
            //else if (actionType == "pswd")
            //{
            //    foreach (var owner in DbSets.Where(u => ids.Contains(u.ID)).ToList())
            //    {
            //        await CarOwnerService.SetPasswordAsync(ParkDB, owner, "123456");
            //    }
            //    ShowNotify("已重设密码为123456");
            //    await DB.SaveChangesAsync();
            //}
            OtherPostBack(actionType,ids);
            await OtherPostBackAsync(actionType,ids);
            return await LoadGrid(Grid1_fields, Grid1_pageIndex, Grid1_sortField, Grid1_sortDirection, ttbSearchMessage, ddlGridPageSize, actionType);
        }
        protected virtual void OtherPostBack(string actionType, List<int> ids)
        {

        }
        protected async virtual Task OtherPostBackAsync(string actionType,List<int> ids)
        {

        }
        protected async Task<IActionResult> LoadGrid(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection, string ttbSearchMessage, int ddlGridPageSize, string actionType)
        {
            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };

            var users = await GetDataAsync(pagingInfo, ttbSearchMessage);
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

        protected abstract Task<IEnumerable<T>> GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage);


    }
}