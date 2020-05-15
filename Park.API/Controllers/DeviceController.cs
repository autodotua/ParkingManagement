using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Park.Service;


namespace Park.API.Controllers
{
    /// <summary>
    /// 为停车场设备提供API
    /// </summary>
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
        /// <summary>
        /// 由进场摄像头发起的车辆进场请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("enter")]
        public async Task<ResponseData<EnterResult>> PostEnterAsync([FromBody] ParkingGateRequest request)
        {
            string licensePlate = request.LicensePlate;
            string token = request.Token;
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
        /// <summary>
        /// 由离场摄像头发起的离场请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("leave")]
        public async Task<ResponseData<LeaveResult>> PostLeaveAsync([FromBody] ParkingGateRequest request)
        {
            string licensePlate = request.LicensePlate;
            string token = request.Token;
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
        /// <summary>
        /// 由车位上放传感器发起的更新车位占用信息的请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ps")]
        public async Task<ResponseData<bool>> PostParkingSpaceStatusAsync([FromBody] SensorRequest request)
        {
            string token = request.Token;
            bool hasCar = request.HasCar;
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
    /// <summary>
    /// 传感器请求体
    /// </summary>
    public class SensorRequest
    {
        public string Token { get; set; }
        public bool HasCar { get; set; }
    }
    /// <summary>
    /// 门卫请求体
    /// </summary>
    public class ParkingGateRequest
    {
        public string Token { get; set; }
        /// <summary>
        /// 摄像头识别到的车牌号
        /// </summary>
        public string LicensePlate { get; set; }
    }
}
