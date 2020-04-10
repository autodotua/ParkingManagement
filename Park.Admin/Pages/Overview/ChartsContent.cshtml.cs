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

namespace Park.Admin.Pages.Overview
{
    public class ChartsContentModel : BaseModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            var recentParkCount = await StatisticsService.GetRecentParkCount(ParkDB, 5);
            ViewBag.RecentParkCountX = recentParkCount.Select(p => p.Key.ToShortDateString());
            ViewBag.RecentParkCountY = recentParkCount.Select(p => p.Value);

            return Page();
        }
 
    }
}