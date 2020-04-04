using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Park.Admin.Models;
using Park.Core.Models;
using Park.Core.Service;

namespace Park.Admin.Pages.Parking
{

    public class ParkingSpaceListModel : BaseModel
    {

        public IEnumerable<ParkingSpace> ParkingSpaces { get; set; }
        public List<ParkArea> ParkAreas { get; set; }

        public int CurrentParkAreaID { get; set; }

        public async Task OnGetAsync()
        {
            ParkAreas = await ParkDB.ParkAreas.ToListAsync();
            //if (ParkAreas.Count > 0)
            //{
            //    CurrentParkAreaID = ParkAreas[0].ID;
            //}
        }

        public async Task<IActionResult> OnPostGridParkArea_RowSelectAsync(string rowId)
        {
            await ParkingSpaceService.ImportFromJsonAsync(ParkDB, @"[
    {
        ""Name"":""停车场"",
        ""Width"":135,
        ""Length"":55,
        ""ParkingSpaces"":[
            {
                ""ID"":0,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":3.5,
                ""Y"":8.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":0
            },
            {
                ""ID"":1,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":9,
                ""Y"":9.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":0
            },
            {
                ""ID"":2,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":14,
                ""Y"":12.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":0
            },
            {
                ""ID"":3,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":9,
                ""Y"":19.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":45
            },
            {
                ""ID"":4,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":27.5,
                ""Y"":25,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":-30
            },
            {
                ""ID"":5,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":36.5,
                ""Y"":6.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":45
            },
            {
                ""ID"":6,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":40.5,
                ""Y"":16,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":45
            },
            {
                ""ID"":7,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":36.5,
                ""Y"":26.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":45
            },
            {
                ""ID"":8,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":12.5,
                ""Y"":28.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":45
            },
            {
                ""ID"":9,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":16.5,
                ""Y"":6.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":45
            },
            {
                ""ID"":10,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":23.5,
                ""Y"":3.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":-60
            },
            {
                ""ID"":11,
                ""Class"":null,
                ""ParkAreaID"":0,
                ""ParkArea"":null,
                ""HasCar"":false,
                ""X"":46,
                ""Y"":9.5,
                ""Width"":4.5,
                ""Height"":2.5,
                ""RotateAngle"":-60
            }
        ],
        ""Aisles"":[
            {
                ""X1"":0,
                ""Y1"":0,
                ""X2"":0,
                ""Y2"":0,
                ""ID"":0,
                ""Class"":null
            },
            {
                ""X1"":0,
                ""Y1"":0,
                ""X2"":0,
                ""Y2"":0,
                ""ID"":1,
                ""Class"":null
            },
            {
                ""X1"":0,
                ""Y1"":0,
                ""X2"":0,
                ""Y2"":0,
                ""ID"":2,
                ""Class"":null
            },
            {
                ""X1"":0,
                ""Y1"":0,
                ""X2"":0,
                ""Y2"":0,
                ""ID"":3,
                ""Class"":null
            },
            {
                ""X1"":0,
                ""Y1"":0,
                ""X2"":0,
                ""Y2"":0,
                ""ID"":4,
                ""Class"":null
            },
            {
                ""X1"":0,
                ""Y1"":0,
                ""X2"":0,
                ""Y2"":0,
                ""ID"":5,
                ""Class"":null
            },
            {
                ""X1"":0,
                ""Y1"":0,
                ""X2"":0,
                ""Y2"":0,
                ""ID"":6,
                ""Class"":null
            },
            {
                ""X1"":29,
                ""Y1"":9,
                ""X2"":36,
                ""Y2"":23,
                ""ID"":7,
                ""Class"":null
            },
            {
                ""X1"":23,
                ""Y1"":25.5,
                ""X2"":23,
                ""Y2"":12,
                ""ID"":8,
                ""Class"":null
            },
            {
                ""X1"":7,
                ""Y1"":3,
                ""X2"":18,
                ""Y2"":16.5,
                ""ID"":9,
                ""Class"":null
            },
            {
                ""X1"":14,
                ""Y1"":24.5,
                ""X2"":33.5,
                ""Y2"":13,
                ""ID"":10,
                ""Class"":null
            }
        ]
    }
]");
            int id = int.Parse(rowId);
            //ParkArea parkArea =await ParkDB.ParkAreas.Include(p=>p.ParkingSpaces).SingleOrDefaultAsync(p=>p.ID==id);
            ParkArea parkArea = await ParkDB.ParkAreas.Include(p => p.ParkingSpaces).FirstAsync();
            ParkingSpaces = parkArea.ParkingSpaces;
            UIHelper.Grid("grdParkingSpace").DataSource(ParkingSpaces);

            var map = ParkingSpaceService.GetMap(ParkDB, parkArea);
            MemoryStream ms = new MemoryStream();
            map.Save(ms, ImageFormat.Png);
            byte[] byteImage = ms.ToArray();
            var SigBase64 = Convert.ToBase64String(byteImage);

            UIHelper.Image("imgMap").ImageUrl("data:image/png;base64," + SigBase64);

            return UIHelper.Result();
        }



        private async Task<IEnumerable<ParkingSpace>> ParkingSpaceList_GetDataAsync()
        {
            IQueryable<ParkingSpace> q = ParkDB.ParkingSpaces;


            return await q.ToListAsync();
        }

        public async Task<IActionResult> OnPostParkingSpaceList_DoPostBackAsync(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, string rblEnableStatus, int ddlGridPageSize, string actionType, int[] deletedRowIDs)
        {
            List<int> ids = new List<int>();
            if (deletedRowIDs != null)
            {
                ids.AddRange(deletedRowIDs);
            }

            var ttbSearchMessageUI = UIHelper.TwinTriggerBox("ttbSearchMessage");
            if (actionType == "trigger1")
            {
                ttbSearchMessageUI.Text(String.Empty);
                ttbSearchMessageUI.ShowTrigger1(false);

                // 清空传入的搜索值
                ttbSearchMessage = String.Empty;
            }
            else if (actionType == "trigger2")
            {
                ttbSearchMessageUI.ShowTrigger1(true);
            }
            else if (actionType == "delete")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreUserDelete"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                ParkDB.CarOwners.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => ParkDB.CarOwners.Remove(u));
                await ParkDB.SaveChangesAsync();
            }
            else if (actionType == "enable")
            {
                ParkDB.CarOwners.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => u.Enabled = true);
                await DB.SaveChangesAsync();
            }
            else if (actionType == "pswd")
            {
                foreach (var owner in ParkDB.CarOwners.Where(u => ids.Contains(u.ID)).ToList())
                {
                    await CarOwnerService.SetPasswordAsync(ParkDB, owner, "123456");
                }
                ShowNotify("已重设密码为123456");
                await DB.SaveChangesAsync();
            }
            else if (actionType == "disable")
            {
                // 在操作之前进行权限检查
                if (!CheckPower("CoreUserEdit"))
                {
                    CheckPowerFailWithAlert();
                    return UIHelper.Result();
                }

                DB.Users.Where(u => ids.Contains(u.ID)).ToList().ForEach(u => u.Enabled = false);
                await DB.SaveChangesAsync();
            }

            return await LoadGrid(Grid1_fields, Grid1_pageIndex, Grid1_sortField, Grid1_sortDirection, ttbSearchMessage, ddlGridPageSize, actionType);
        }

        private async Task<IActionResult> LoadGrid(string[] Grid1_fields, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection, string ttbSearchMessage, int ddlGridPageSize, string actionType)
        {
            var grid1UI = UIHelper.Grid("Grid1");
            var pagingInfo = new PagingInfoViewModel
            {
                SortField = Grid1_sortField,
                SortDirection = Grid1_sortDirection,
                PageIndex = Grid1_pageIndex,
                PageSize = ddlGridPageSize
            };

            var users = await ParkingSpaceList_GetDataAsync();
            // 1. 设置总项数
            grid1UI.RecordCount(pagingInfo.RecordCount);
            // 2. 设置每页显示项数
            if (actionType == "changeGridPageSize")
            {
                grid1UI.PageSize(ddlGridPageSize);
            }
            // 3.设置分页数据
            grid1UI.DataSource(users, Grid1_fields);

            return UIHelper.Result();
        }

        public async Task<IActionResult> OnPostBtnSubmit_Click(string[] Grid1_fields, JArray Grid1_modifiedData, int Grid1_pageIndex, string Grid1_sortField, string Grid1_sortDirection,
            string ttbSearchMessage, string rblEnableStatus, int ddlGridPageSize, string actionType, int[] deletedRowIDs)
        {

            var a = Grid1_modifiedData.ToString();
            foreach (JObject modifiedRow in Grid1_modifiedData)
            {
                string status = modifiedRow.Value<string>("status");
                int rowId = Convert.ToInt32(modifiedRow.Value<string>("id"));

                if (status == "modified")
                {
                    var owner = ParkDB.CarOwners.Find(rowId);
                    owner.Enabled = modifiedRow["values"]["Enabled"].Value<bool>();
                    ParkDB.Entry(owner).State = EntityState.Modified;
                }
            }
            await ParkDB.SaveChangesAsync();
            //UIHelper.Grid("Grid1").DataSource(source, Grid1_fields);
            //UIHelper.Label("labResult").Text(String.Format("用户修改的数据：<pre>{0}</pre>", Grid1_modifiedData.ToString(Newtonsoft.Json.Formatting.Indented)), false);
            ShowNotify("数据保存成功！");

            return await LoadGrid(Grid1_fields, Grid1_pageIndex, Grid1_sortField, Grid1_sortDirection, ttbSearchMessage, ddlGridPageSize, actionType);



            //return UIHelper.Result();
        }

    }
}