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
    [CheckPower(Name = "CoreDeptView")]
    public class DeptModel : BaseAdminModel
    {
        public IEnumerable<Dept> Depts { get; set; }

        public bool PowerCoreDeptNew { get; set; }
        public bool PowerCoreDeptEdit { get; set; }
        public bool PowerCoreDeptDelete { get; set; }

        public void OnGet()
        {
            Depts = Dept_LoadData();
        }

        private IEnumerable<Dept> Dept_LoadData()
        {
            PowerCoreDeptNew = CheckPower("CoreDeptNew");
            PowerCoreDeptEdit = CheckPower("CoreDeptEdit");
            PowerCoreDeptDelete = CheckPower("CoreDeptDelete");

            return DeptHelper.Depts;
        }

        public async Task<IActionResult> OnPostDept_DoPostBackAsync(string[] Grid1_fields, string actionType, int? deletedRowID)
        {
            if (actionType == "delete")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreDeptDelete"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                int userCount = await DB.Users.Where(u => u.Dept.ID == deletedRowID.Value).CountAsync();
                if (userCount > 0)
                {
                    Alert.ShowInTop("删除失败！需要先清空属于此部门的用户！");
                    return UIHelper.Result();
                }

                int childCount = await DB.Depts.Where(d => d.Parent.ID == deletedRowID.Value).CountAsync();
                if (childCount > 0)
                {
                    Alert.ShowInTop("删除失败！请先删除子部门！");
                    return UIHelper.Result();
                }

                var dept = await DB.Depts.Where(d => d.ID == deletedRowID.Value).FirstOrDefaultAsync();
                DB.Depts.Remove(dept);
                await DB.SaveChangesAsync();
            }


            DeptHelper.Reload();
            UIHelper.Grid("Grid1").DataSource(DeptHelper.Depts, Grid1_fields);

            return UIHelper.Result();
        }
    }
}