using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.IO;
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

namespace Park.Admin.Pages.People
{
    public class ParkAreaListModel : GridBaseModel<ParkArea>
    {
        public IEnumerable<ParkArea> ParkAreas { get; set; }

        public override DbSet<ParkArea> DbSets => ParkDB.ParkAreas;

        public async Task OnGetAsync()
        {
            PagingInfo = new PagingInfoViewModel
            {
                PageIndex = 0,
                PageSize = ConfigHelper.PageSize
            };
            ParkAreas = await GetDataAsync(PagingInfo, string.Empty);
        }

        public async Task<IActionResult> OnPostGridParkArea_RowSelectAsync(string rowId)
        {
            int id = int.Parse(rowId);
            //ParkArea parkArea =await ParkDB.ParkAreas.Include(p=>p.ParkingSpaces).SingleOrDefaultAsync(p=>p.ID==id);
            ParkArea parkArea = await ParkDB.ParkAreas.Include(p => p.ParkingSpaces)
                .Include(p => p.Aisles)
                .Include(p => p.Walls).FirstAsync(p => p.ID == id);
      

            var map = ParkingSpaceService.GetMap(ParkDB, parkArea);
            MemoryStream ms = new MemoryStream();
            map.Save(ms, ImageFormat.Png);
            byte[] byteImage = ms.ToArray();
            var SigBase64 = Convert.ToBase64String(byteImage);

            UIHelper.Image("imgMap").ImageUrl("data:image/png;base64," + SigBase64);

            return UIHelper.Result();
        }

        protected override async Task<IEnumerable<ParkArea>> GetDataAsync(PagingInfoViewModel pagingInfo, string ttbSearchMessage)
        {
            IQueryable<ParkArea> q = ParkDB.ParkAreas;

            string searchText = ttbSearchMessage?.Trim();
            if (!String.IsNullOrEmpty(searchText))
            {
                q = q.Where(u => u.Name.Contains(searchText));
            }
         

            pagingInfo.RecordCount = await q.CountAsync();

            // 排列和数据库分页
            q = SortAndPage(q, pagingInfo);

            return (await q.Include(p => p.ParkingSpaces).ToListAsync());
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
                    var ps = ParkDB.ParkAreas.Find(rowId);
                    ps.GateTokens = modifiedRow["values"]["GateTokens"].Value<string>();
                    ParkDB.Entry(ps).State = EntityState.Modified;
                }
            }
            await ParkDB.SaveChangesAsync();
            ShowNotify("数据保存成功！");

            return await LoadGrid(Grid1_fields, Grid1_pageIndex, Grid1_sortField, Grid1_sortDirection, ttbSearchMessage, ddlGridPageSize, actionType);
        }

    }
}