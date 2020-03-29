using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Park.Admin.Pages.ParkingSpace
{
    public class DesignModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(int id)
        {

            return Page();
        }
    }
}