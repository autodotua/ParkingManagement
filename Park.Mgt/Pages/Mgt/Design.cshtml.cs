using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Park.Mgt.Pages.Mgt
{
    public class DesignModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(int id)
        {

            return Page();
        }
    }
}