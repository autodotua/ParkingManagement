using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Mgt.Models;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Park.Mgt.Pages.Admin
{
    [CheckPower(Name = "CoreTitleNew")]
    public class TitleNewModel : BaseAdminModel
    {
        [BindProperty]
        public Title Title { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostTitleNew_btnSaveClose_ClickAsync()
        {
            if (ModelState.IsValid)
            {
                DB.Titles.Add(Title);
                await DB.SaveChangesAsync();

                // 关闭本窗体（触发窗体的关闭事件）
                ActiveWindow.HidePostBack();
            }

            return UIHelper.Result();
        }
    }
}