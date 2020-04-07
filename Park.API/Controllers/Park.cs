using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Park.Core.Models;
using Park.Core.Service;


namespace Park.API.Controllers
{
    [ApiController]
    [EnableCors("cors")]
    [Route("[controller]")]
    public class ParkController : ControllerBase
    {
        Context db;
        public ParkController()
        {
            db = new Context();
        }

        [HttpPost]
        [Route("enter")]
        public async Task<ResponseData<bool>> PostEnterAsync([FromBody] ParkingGateRequest request)
        { 
            string licensePlate = request.licensePlate; 
            string token = request.token;
            try
            {
                var pa = (await db.ParkAreas.ToListAsync()).FirstOrDefault(p => p.GateTokens.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Contains(token));
                if (pa == null)
                {
                    return new ResponseData<bool>() { Succeed = false, Message = "找不到对应的停车区" };
                }
                return new ResponseData<bool>()
                {
                    Data = await ParkService.EnterAsync(db, licensePlate, pa)
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<bool>() { Succeed = false, Message = ex.Message };
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
            string token= request.token;
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
    
    public class ResponseData<T>
    {
        public bool Succeed { get; set; } = true;
        public string Message { get; set; }
        public T Data { get; set; }
    }
    public class Context : ParkContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ParkSQLServer"));
            //base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer(connStr);
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
