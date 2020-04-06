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
    public class TransactionRecordListModel : GridBaseModel<TransactionRecord>
    {
        public IEnumerable<TransactionRecord> TransactionRecords { get; set; }

        public override DbSet<TransactionRecord> DbSets => ParkDB.TransactionRecords;

        public async Task OnGetAsync(int? carOwnerID)
        {
            this.carOwnerID = carOwnerID;
            PagingInfo = new PagingInfoViewModel
            {
                SortField = "Time",
                SortDirection = "DESC",
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };
            TransactionRecords = await GetDataAsync(PagingInfo, string.Empty);
        }

        int? carOwnerID;

        protected override async Task<IEnumerable<TransactionRecord>> GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage)
        {
            IQueryable<TransactionRecord> q = ParkDB.TransactionRecords;

            string searchText = ttbSearchMessage?.Trim();
            //if (!String.IsNullOrEmpty(searchText))
            //{
            //    q = q.Where(u => u.LicensePlate.Contains(searchText));
            //}
            if (carOwnerID.HasValue)
            {
                q = q.Where(p => p.CarOwnerID == carOwnerID.Value);
            }


            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage(q, pagingInfo);

            return (await q.Include(p=>p.CarOwner).ToListAsync());
        }
    }
}