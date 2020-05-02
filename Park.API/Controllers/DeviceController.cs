using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Park.Service;


namespace Park.API.Controllers
{
    [ApiController]
    [EnableCors("cors")]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {
        Context db;
        public DeviceController()
        {
            db = new Context();
        }

        [HttpPost]
        [Route("enter")]
        public async Task<ResponseData<EnterResult>> PostEnterAsync([FromBody] ParkingGateRequest request)
        {
            string licensePlate = request.licensePlate;
            string token = request.token;
            try
            {
                var pa = (await db.ParkAreas.ToListAsync()).FirstOrDefault(p => p.GateTokens.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Contains(token));
                if (pa == null)
                {
                    return new ResponseData<EnterResult>() { Succeed = false, Message = "找不到对应的停车区" };
                }
                return new ResponseData<EnterResult>()
                {
                    Data = await ParkService.EnterAsync(db, licensePlate, pa)
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<EnterResult>() { Succeed = false, Message = ex.Message };
            }
        }
        [HttpPost]
        [Route("leave")]
        public async Task<ResponseData<LeaveResult>> PostLeaveAsync([FromBody] ParkingGateRequest request)
        {
            string licensePlate = request.licensePlate;
            string token = request.token;
            try
            {
                var pa = (await db.ParkAreas.ToListAsync()).FirstOrDefault(p => p.GateTokens.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Contains(token));
                if (pa == null)
                {
                    return new ResponseData<LeaveResult>() { Succeed = false, Message = "找不到对应的停车区" };
                }
                return new ResponseData<LeaveResult>()
                {
                    Data = await ParkService.LeaveAsync(db, licensePlate, pa)
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<LeaveResult>() { Succeed = false, Message = ex.Message };
            }
        }


        [HttpPost]
        [Route("ps")]
        public async Task<ResponseData<bool>> PostParkingSpaceStatusAsync([FromBody] SensorRequest request)
        {
            string token = request.token;
            bool hasCar = request.hasCar;
            try
            {
                var ps = await db.ParkingSpaces.FirstOrDefaultAsync(p => p.SensorToken == token);
                if (ps == null)
                {
                    return new ResponseData<bool>() { Succeed = false, Message = "找不到对应的停车位" };
                }
                ps.HasCar = hasCar;
                await db.SaveChangesAsync();

                return new ResponseData<bool>()
                {
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<bool>() { Succeed = false, Message = ex.Message };
            }
        }

    }
    public class SensorRequest
    {
        public string token { get; set; }
        public bool hasCar { get; set; }
    }
    public class ParkingGateRequest
    {
        public string token { get; set; }
        public string licensePlate { get; set; }
    }
}
