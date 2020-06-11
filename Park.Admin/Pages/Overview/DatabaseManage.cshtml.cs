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
    [CheckPower(Name = "ParkPower")]

    public class DatabaseManageModel : BaseModel
    {
        //public List<ParkArea> ParkAreas { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {

            return Page();
        }
        public async Task<IActionResult> OnPostResetDatabaseAsync()
        {
            await ParkDB.Database.EnsureDeletedAsync();
            await ParkDB.Database.EnsureCreatedAsync();
            ShowNotify("操作成功");
            return UIHelper.Result();
        } 
        public async Task<IActionResult> OnPostResetAdminDatabaseAsync()
        {
            await DB.Database.EnsureDeletedAsync();
            ParkAdminDatabaseInitializer.Initialize(DB);
            ShowNotify("操作成功，请重新登录");
            return UIHelper.Result();
        }
        public async Task<IActionResult> OnPostGenerateTestDataAsync(int count)
        {
            await ParkDatabaseInitializer.GenerateTestDatasAsync(ParkDB,count);
            ShowNotify("操作成功");
            return UIHelper.Result();
        }

    }
}