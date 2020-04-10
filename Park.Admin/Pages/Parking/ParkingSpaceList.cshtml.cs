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
    [CheckPower(Name = "ParkPower")]

    public class ParkingSpaceListModel : GridBaseModel<ParkingSpace>
    {
        private int? parkAreaID;

        public IEnumerable<ParkingSpace> ParkingSpaces { get; set; }


        public override DbSet<ParkingSpace> DbSets => ParkDB.ParkingSpaces;

        public async Task OnGetAsync(int? parkAreaID)
        {
            this.parkAreaID = parkAreaID;
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = "ID",
                SortDirection = "ASC    ",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };

            PagingInfo = pagingInfo;

            ParkingSpaces = (await GetDataAsync(pagingInfo, string.Empty));
        }

    

        protected override async Task<IEnumerable<ParkingSpace>> GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage)
        {
            IQueryable<ParkingSpace> q = ParkDB.ParkingSpaces;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.SensorToken.Contains(searchText));
            }
            if(parkAreaID.HasValue)
            {
                q = q.Where(p => p.ParkAreaID == parkAreaID.Value);
            }
            //pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
           // q = SortAndPage(q, pagingInfo);

            return await q.ToListAsync();
        }


        protected async override Task OtherPostBackAsync(string actionType,List<int> ids)
        {
          
        }
        public async Task<IActionResult> OnPostSaveDataAsync(string[] Grid1_fields, JArray Grid1_modifiedData, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
        string ttbSearchMessage, int ddlGridPageSize, string actionType)
        {

            foreach (JObject modifiedRow in Grid1_modifiedData)
            {
                string status = modifiedRow.Value<string>("status");
                int rowId = Convert.ToInt32(modifiedRow.Value<string>("id"));

                if (status == "modified")
                {
                    var ps = ParkDB.ParkingSpaces.Find(rowId);
                    ps.SensorToken = modifiedRow["values"]["DeviceToken"].Value<string>();
                    ParkDB.Entry(ps).State = EntityState.Modified;
                }
            }
            await ParkDB.SaveChangesAsync();
            ShowNotify("数据保存成功！");

            return await LoadGrid(Grid1_fields, Grid1_pageIndex, Grid1_sortField, Grid1_sortDirection, ttbSearchMessage, ddlGridPageSize, actionType);
        }

    }
}