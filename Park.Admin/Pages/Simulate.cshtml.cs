using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Admin.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Park.Core.Models;
using FineUICore;

namespace Park.Admin.Pages
{
    public class SimulateModel : BaseModel
    {
        //public List<ParkArea> ParkAreas { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            ViewBag.ParkAreas = await ParkDB.ParkAreas.ToListAsync();
            ListItem listItem = new ListItem();

            //if (ParkAreas.Count > 0)
            //{
            //    ViewBag.ParkingSpaces = await LoadParkingSpacesAsync();
            //}

            ViewBag.Cars = new List<string>();
            foreach (var car in (await ParkDB.Cars.ToListAsync()).Select(p => p.LicensePlate))
            {
                ViewBag.Cars.Add(car);
            }
            return Page();
        }
        public async Task<IActionResult> OnPostParkAreaDropDownList_SelectedChangedAsync(string ddlParkArea, string ddlParkArea_text)
        {
            int id = int.Parse(ddlParkArea);
           var pa= await ParkDB.ParkAreas.Include(p => p.ParkingSpaces).FirstOrDefaultAsync() ;
            UIHelper.DropDownList("ddlParkingSpace").LoadData(await LoadParkingSpacesAsync(id));
            UIHelper.DropDownList("ddlParkAreaTokens").DataSource(pa.GateTokens.Split(new string[] { ";" },StringSplitOptions.RemoveEmptyEntries));
            return UIHelper.Result();
        }
        public async Task<ListItem[]> LoadParkingSpacesAsync(int? id = null)
        {
            IQueryable<ParkArea> q = ParkDB.ParkAreas.Include(p => p.ParkingSpaces);
     
                q = q.Where(p => p.ID == id.Value);
            var parkingSpaces = (await q.FirstOrDefaultAsync()).ParkingSpaces;
            return parkingSpaces.Select(p => new ListItem
            {
                Value = p.SensorToken.ToString(),
                Text = "停车位" + p.ID
            }).ToArray();

        }

        public async Task<IActionResult> OnPostParkingSpaceDropDownList_SelectedChangedAsync(string ddlParkArea, string ddlParkArea_text)
        {
            return UIHelper.Result();
        }
    }
}