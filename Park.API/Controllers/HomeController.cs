using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Park.Models;
using Park.Service;

namespace Park.API.Controllers
{
    /// <summary>
    /// 为Park.Mobile的主页提供相关信息
    /// </summary>
    public class HomeController : ParkControllerBase
    {
        /// <summary>
        /// 获取某一个停车场的当前车位地图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ParkImage/{id}")]
        public async Task<IActionResult> ParkImageAsync(int id)
        {
            ParkArea park = await db.ParkAreas.Where(p=>p.ID==id)
                .Include(p=>p.ParkingSpaces)
                .Include(p=>p.Walls)
                .Include(p=>p.Aisles)
                .FirstOrDefaultAsync();
            if(park==null)
            {
                return NotFound();
            }
           var image= ParkingSpaceService.GetMap(db, park);

            using MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            ActionResult result = Ok();
            return File(ms.ToArray(), "image/png");
        }
        /// <summary>
        /// 获取主页所有需要的信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Index")]
        public async Task<ResponseData<OverviewResponse>> IndexAsync([FromBody] UserToken request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<OverviewResponse>() { Succeed = false, Message = "用户验证失败" };
            }
            OverviewResponse response = new OverviewResponse();
            foreach (var car in await db.Cars.Where(p => p.CarOwnerID == request.UserID).ToListAsync())
            {
                int recordCount = await db.ParkRecords.CountAsync(p => p.CarID == car.ID);
                response.Cars.Add(new { car.LicensePlate, Records = recordCount, car.ID });
            }
            TransactionRecord transaction = await db.TransactionRecords
                .LastOrDefaultRecordAsync(p => p.Time, p => p.CarOwnerID == request.UserID);
            response.Balance = transaction==null?0:transaction.Balance;
            response.ExpireTime  = transaction == null ? System.DateTime.MinValue : transaction.ExpireTime;
            response.Parks = await db.ParkAreas.ToListAsync();
            return new ResponseData<OverviewResponse>() { Data = response };
        }

    }
    /// <summary>
    /// 概览的回应
    /// </summary>
    public class OverviewResponse
    {
        /// <summary>
        /// 车辆信息。由于不需要太多信息，因为车辆信息为动态的。
        /// 其中包括车牌号和停车次数
        /// </summary>
        public List<dynamic> Cars { get; set; } = new List<dynamic>();
        /// <summary>
        /// 余额
        /// </summary>
        public double Balance { get; set; }
        /// <summary>
        /// 月租到期时间
        /// </summary>
        public System.DateTime ExpireTime { get; set; }
        /// <summary>
        /// 停车区信息
        /// </summary>
        public List<ParkArea> Parks { get; set; }
    }
}
