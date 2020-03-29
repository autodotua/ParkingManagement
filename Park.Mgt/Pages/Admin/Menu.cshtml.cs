using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Mgt.Models;

using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Park.Mgt.Pages.Admin
{
    [CheckPower(Name = "CoreMenuView")]
    public class MenuModel : BaseAdminModel
    {
        public IEnumerable<Models.Menu> Menus { get; set; }

        public bool PowerCoreMenuNew { get; set; }
        public bool PowerCoreMenuEdit { get; set; }
        public bool PowerCoreMenuDelete { get; set; }

        public void OnGet()
        {
            Menu_LoadData();

            Menus = MenuHelper.Menus;
        }

        private void Menu_LoadData()
        {
            PowerCoreMenuNew = CheckPower("CoreMenuNew");
            PowerCoreMenuEdit = CheckPower("CoreMenuEdit");
            PowerCoreMenuDelete = CheckPower("CoreMenuDelete");
        }

        public async Task<IActionResult> OnPostMenu_DeleteRowAsync(string[] Grid1_fields, int deletedRowID)
        {
            // 在操作之前进行权限检查
            if (!CheckPower("CoreMenuDelete"))
            {
                CheckPowerFailWithAlert();
                return UIHelper.Result();
            }

            int childCount = await DB.Menus.Where(m => m.Parent.ID == deletedRowID).CountAsync();
            if (childCount > 0)
            {
                Alert.ShowInTop("删除失败！请先删除子菜单！");
                return UIHelper.Result();
            }


            var menu = await DB.Menus.Where(m => m.ID == deletedRowID).FirstOrDefaultAsync();
            DB.Menus.Remove(menu);
            await DB.SaveChangesAsync();

            MenuHelper.Reload();
            UIHelper.Grid("Grid1").DataSource(MenuHelper.Menus, Grid1_fields);

            return UIHelper.Result();
        }

        public IActionResult OnPostMenu_Window1_Close(string[] Grid1_fields)
        {
            MenuHelper.Reload();
            UIHelper.Grid("Grid1").DataSource(MenuHelper.Menus, Grid1_fields);

            return UIHelper.Result();
        }
    }
}