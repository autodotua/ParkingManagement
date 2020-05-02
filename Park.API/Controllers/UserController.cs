using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Park.Models;
using Park.Service;


namespace Park.API.Controllers
{
    [ApiController]
    [EnableCors("cors")]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        Context db;
        public UserController()
        {
            db = new Context();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ResponseData<LoginResult>> LoginAsync([FromBody] LoginRequest request)
        {
            var a = await db.CarOwners.FirstAsync();
            var result = await CarOwnerService.Login(db, request.Username, request.Password);
            switch (result.Type)
            {
                case LoginOrRegisterResultType.Succeed:
                    var r = new LoginResult(result.CarOwner);
                    HttpContext.Session.SetInt32("user", r.CarOwner.ID);
                    return new ResponseData<LoginResult>()
                    {
                        Message = "登陆成功",
                        Data = r
                    };
                case LoginOrRegisterResultType.Empty:
                    return new ResponseData<LoginResult>()
                    {
                        Message = "用户名或密码为空",
                    };
                case LoginOrRegisterResultType.Wrong:
                    return new ResponseData<LoginResult>()
                    {
                        Message = "用户名或密码错误",
                    };
                default:
                    throw new NotImplementedException();
            }
        }
        [HttpPost]
        [Route("Register")]
        public async Task<ResponseData<LoginResult>> RegisterAsync([FromBody] LoginRequest request)
        {
            var a = await db.CarOwners.FirstAsync();
            var result = await CarOwnerService.Register(db, request.Username, request.Password);
            switch (result.Type)
            {
                case LoginOrRegisterResultType.Succeed:
                    var r = new LoginResult(result.CarOwner);
                    HttpContext.Session.SetInt32("user", r.CarOwner.ID);
                    return new ResponseData<LoginResult>()
                    {
                        Message = "注册成功",
                        Data = r
                    };
                case LoginOrRegisterResultType.Existed:
                    return new ResponseData<LoginResult>()
                    {
                        Message = "用户名已存在",
                    };
                default:
                    throw new NotImplementedException();
            }
        }
        [HttpPost]
        [Route("Car")]
        public async Task<ResponseData<bool>> CarAsync([FromBody] CarRequest request)
        {
            var a = HttpContext.Request.Cookies.ToArray();
            if (!HttpContext.Session.GetInt32("user").HasValue)
            {
                return new ResponseData<bool>() { Succeed = false, Message = "用户验证失败" };
            }
            int carOwnerID = HttpContext.Session.GetInt32("user").Value;

            switch (request.Type)
            {
                case "add":
                    var car = request.Car;
                    car.CarOwnerID = carOwnerID;
                    car.Enabled = true;
                    db.Cars.Add(car);
                    await db.SaveChangesAsync();
                    return new ResponseData<bool>() { Data = true };
                case "delete":
                    db.Cars.Remove(request.Car);
                    await db.SaveChangesAsync();
                    return new ResponseData<bool>() { Data = true };
                default:
                    throw new NotImplementedException();
            }
        }

    }
    public class LoginRequest
    {
        public string Password { get; set; }
        public string Username { get; set; }
    }
    public class CarRequest : RequestWithTokenBase
    {
        public string Type { get; set; }//add/edit/delete
        public Car Car { get; set; }
    }
    public class LoginResult
    {
        public LoginResult(CarOwner carOwner)
        {
            CarOwner = carOwner;
            carOwner.Password = null;
            Token = Guid.NewGuid().ToString("N");
        }

        public CarOwner CarOwner { get; set; }
        public string Token { get; set; }
    }

    public abstract class RequestWithTokenBase
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}
