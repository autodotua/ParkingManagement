using Park.Admin.Models;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CoreTitleView")]
    public class UserSelectTitleModel : BaseAdminModel
    {
        public IEnumerable<Title> Titles { get; set; }

        public string[] TitleSelectedValueArray { get; set; }


        public async Task OnGetAsync(string ids)
        {
            ids ??= "";

            TitleSelectedValueArray = ids.Split(',');

            Titles = await DB.Titles.AsNoTracking().ToListAsync();
        }
    }
}