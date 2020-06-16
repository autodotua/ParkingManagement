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
    [CheckPower(Name = "ParkPower")]

    public class ImportParkModel : BaseModel
    {

        public async Task OnGetAsync()
        {

        }

        public async Task<IActionResult> OnPostImportAsync(string txt)
        {
            try
            {
               await ParkingSpaceService.ImportFromJsonAsync(ParkDB, txt);
                ShowNotify("导入成功");
            }
            catch(Exception ex)
            {
                ShowNotify("导入失败："+ex.Message);
            }
            ActiveWindow.HidePostBack();
            return UIHelper.Result();
        }
    }
}