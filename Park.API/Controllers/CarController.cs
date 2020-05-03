using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Park.Models;


namespace Park.API.Controllers
{
    public class CarController : ParkControllerBase
    {
        public CarController()
        {
        }
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
    public class CarRequest : UserToken
    {
        public string Type { get; set; }//add/edit/delete/detail
        public int CarID { get; set; }
        public string LicensePlate { get; set; }
    }
}
