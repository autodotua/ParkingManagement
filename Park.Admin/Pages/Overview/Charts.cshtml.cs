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
    public class ChartsModel : BaseModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

    }
}