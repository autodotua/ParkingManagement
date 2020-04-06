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
using Park.Core.Models;

namespace Park.Admin.Pages.CarAndOwner
{
    public class ParkRecordListModel: GridBaseModel<ParkRecord>
    {
        public IEnumerable<ParkRecord> ParkRecords { get; set; }

        public override DbSet<ParkRecord> DbSets => ParkDB.ParkRecords;

        public async Task OnGetAsync(int? carOwnerID)
        {
            this.carID = carOwnerID;
            PagingInfo = new PagingInfoViewModel
            {
                SortField = "LeaveTime",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };
            ParkRecords = await GetDataAsync(PagingInfo, string.Empty);
        }

        int? carID;

        protected override async Task<IEnumerable<ParkRecord>> GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage)
        {
            IQueryable<ParkRecord> q = ParkDB.ParkRecords;

            string searchText = ttbSearchMessage?.Trim();
            //if (!String.IsNullOrEmpty(searchText))
            //{
            //    q = q.Where(u => u.LicensePlate.Contains(searchText));
            //}
            if (carID.HasValue)
            {
                q = q.Where(p => p.CarID == carID.Value);
            }


            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage(q, pagingInfo);

            return (await q.Include(p => p.ParkArea).Include(p=>p.Car).Include(p=>p.Car.CarOwner).ToListAsync());
        }
    }
}