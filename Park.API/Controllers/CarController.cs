using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Park.Models;


namespace Park.API.Controllers
{
    /// <summary>
    /// 为Park.Mobile提供汽车相关API
    /// </summary>
    public class CarController : ParkControllerBase
    {
        public CarController()
        {
        }
        /// <summary>
        /// 调整车辆相关信息。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Modify")]
        public async Task<ResponseData<object>> ModifyAsync([FromBody] CarRequest request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<object>() { Succeed = false, Message = "用户验证失败" };
            }
            int carOwnerID = request.UserID;
            Car car = null;
            switch (request.Type)
            {
                case "add":
                    car = new Car()
                    {
                        CarOwnerID = carOwnerID,
                        Enabled = true,
                        LicensePlate = request.LicensePlate
                    };

                    db.Cars.Add(car);
                    await db.SaveChangesAsync();
                    return new ResponseData<object>() { Data = true };
                case "detail":
                    car = await db.Cars.Include(p => p.ParkRecords).ThenInclude(p => p.ParkArea)
                          .Include(p => p.ParkRecords).ThenInclude(p => p.TransactionRecord)
                          .FirstOrDefaultAsync(p => p.ID == request.CarID && p.CarOwnerID == request.UserID);

                    return new ResponseData<object>() { Data = car };
                case "delete":
                    car = await db.Cars.FirstOrDefaultAsync(p => p.ID == request.CarID
                     && p.CarOwnerID == request.UserID);
                    if (car != null)
                    {
                        db.Cars.Remove(car);
                        await db.SaveChangesAsync();
                    }
                    return new ResponseData<object>() { Data = true };
                default:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 获取车辆信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Index")]
        public async Task<ResponseData<List<dynamic>>> IndexAsync([FromBody] UserToken request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<List<dynamic>>() { Succeed = false, Message = "用户验证失败" };
            }
            var cars = new List<dynamic>();
            foreach (var car in await db.Cars.Where(p => p.CarOwnerID == request.UserID).ToListAsync())
            {
                int recordCount = await db.ParkRecords.CountAsync(p => p.CarID == car.ID);
                cars.Add(new { car.LicensePlate, Records = recordCount, car.ID });
            }
            return new ResponseData<List<dynamic>>(cars);
        }

    }
    /// <summary>
    /// 汽车请求
    /// </summary>
    public class CarRequest : UserToken
    {
        /// <summary>
        /// 请求类型，分为add（新增汽车）、edit（编辑汽车）、delete（删除汽车）
        /// </summary>
        public string Type { get; set; }//add/edit/delete/detail
        /// <summary>
        /// 汽车ID
        /// </summary>
        public int CarID { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicensePlate { get; set; }
    }
}
