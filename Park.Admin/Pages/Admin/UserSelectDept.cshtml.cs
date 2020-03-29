using Park.Admin.Models;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CoreDeptView")]
    public class UserSelectDeptModel : BaseAdminModel
    {
        public IEnumerable<Dept> Depts { get; set; }

        public string DeptSelectedRowID { get; set; }

        public async Task OnGetAsync(string ids)
        {
            ids ??= "";

            DeptSelectedRowID = ids;

            Depts = await DB.Depts.AsNoTracking().ToListAsync();
        }
    }
}