using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Admin.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Park.Models;
using FineUICore;
using Park.Service;
using System.IO;
using System.Drawing.Imaging;

namespace Park.Admin.Pages.Overview
{
    public class ChartsContentModel : BaseModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            var recentParkCount = await StatisticsService.GetRecentDaysParkCount(ParkDB, 5, true);
            ViewBag.DaysEnter = recentParkCount.Select(p => $"['{p.Key}',{p.Value}]");

            var recentHoursEnterParkCount = await StatisticsService.GetRecentHoursParkCount(ParkDB, true);
            ViewBag.HoursEnter = recentHoursEnterParkCount.Select(p => $"['{p.Key}',{p.Value}]");

            recentParkCount = await StatisticsService.GetRecentDaysParkCount(ParkDB, 5, false);
            ViewBag.DaysLeave = recentParkCount.Select(p => $"['{p.Key}',{p.Value}]");

            recentHoursEnterParkCount = await StatisticsService.GetRecentHoursParkCount(ParkDB, false);
            ViewBag.HoursLeave = recentHoursEnterParkCount.Select(p => $"['{p.Key}',{p.Value}]");

            ViewBag.Status = await StatisticsService.GetParkStatusAsync(ParkDB);

            List<ParkArea> parkAreas = await ParkDB.ParkAreas.Include(p => p.ParkingSpaces)
                .Include(p => p.Aisles)
                .Include(p => p.Walls)
                .ToListAsync();
            foreach (var p in parkAreas)
            {
                var map = ParkingSpaceService.GetMap(ParkDB, p);
                using MemoryStream ms = new MemoryStream();
                map.Save(ms, ImageFormat.Png);
                p.Map = ms.ToArray();
            }
            ViewBag.Parks = parkAreas;
            return Page();
        }



    }
}