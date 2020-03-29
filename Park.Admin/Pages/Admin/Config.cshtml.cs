using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Park.Admin.Models;

using FineUICore;
using Newtonsoft.Json.Linq;

namespace Park.Admin.Pages.Admin
{
    [CheckPower(Name = "CoreConfigView")]
    public class ConfigModel : BaseAdminModel
    {
        public bool PowerCoreConfigEdit { get; set; }
        public string HelpListText { get; set; }

        public async Task OnGetAsync()
        {
            await Config_LoadDataAsync();
        }

        private async Task Config_LoadDataAsync()
        {
            PowerCoreConfigEdit = CheckPower("CoreConfigEdit");

            await Task.Run(() =>
            {
                JSBeautifyLib.JSBeautify jsb = new JSBeautifyLib.JSBeautify(ConfigHelper.HelpList, new JSBeautifyLib.JSBeautifyOptions());
                HelpListText = jsb.GetResult();
            });
        }

        public IActionResult OnPostConfig_btnSave_OnClick(int ddlPageSize, string tbxHelpList)
        {
            // 在操作之前进行权限检查
            if (!CheckPower("CoreConfigEdit"))
            {
                CheckPowerFailWithAlert();
                return UIHelper.Result();
            }

            try
            {
                JArray.Parse(tbxHelpList.Trim());
            }
            catch (Exception)
            {
                UIHelper.TextArea("tbxHelpList").MarkInvalid("格式不正确，必须是JSON字符串！");
                return UIHelper.Result();
            }

            //string title = tbxTitle.Trim();
            //if (title.Length > 100)
            //{
            //    title = title.Substring(0, 100);
            //}

            //ConfigHelper.Title = title;
            ConfigHelper.PageSize = ddlPageSize;
            ConfigHelper.HelpList = tbxHelpList.Trim();
            ConfigHelper.SaveAll();

            FineUICore.PageContext.RegisterStartupScript("top.window.location.reload(false);");

            return UIHelper.Result();
        }
    }
}