using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Park.Admin.Models;
using Park.Models;

namespace Park.Admin.Pages.CarAndOwner
{
    [CheckPower(Name = "CarAndOwnerPower")]
    public class CarListModel:GridBaseModel<Car>
    {
        public IEnumerable<Car> Cars { get; set; }

        public override DbSet<Car> DbSets => ParkDB.Cars;

        public async Task OnGetAsync(int? carOwnerID)
        {
            this.carOwnerID = carOwnerID;
            PagingInfo = new PagingInfoViewModel
            {
                SortField = "LicensePlate",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };
            Cars = await GetDataAsync(PagingInfo, string.Empty);
        }

        int? carOwnerID;

        protected override async Task<IEnumerable<Car>> GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage)
        {
            IQueryable<Car> q = ParkDB.Cars;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.LicensePlate.Contains(searchText));
            }
            if (carOwnerID.HasValue)
            {
                q = q.Where(p => p.CarOwnerID == carOwnerID.Value);
            }


            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage(q, pagingInfo);

            return (await q.Include(p => p.ParkRecords).Include(p => p.CarOwner).ToListAsync());
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
                    var car = ParkDB.Cars.Find(rowId);
                    car.Enabled = modifiedRow["values"]["Enabled"].Value<bool>();
                    ParkDB.Entry(car).State = EntityState.Modified;
                }
            }
            await ParkDB.SaveChangesAsync();
            ShowNotify("数据保存成功！");

            return await LoadGrid(Grid1_fields, Grid1_pageIndex, Grid1_sortField, Grid1_sortDirection, ttbSearchMessage, ddlGridPageSize, actionType);
        }

    }
}