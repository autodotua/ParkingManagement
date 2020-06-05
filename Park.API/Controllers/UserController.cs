using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Park.Models;
using Park.Service;


namespace Park.API.Controllers
{/// <summary>
/// 为Park.Mobile提供用户相关API
/// </summary>
    public class UserController : ParkControllerBase
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public async Task<ResponseData<LoginResult>> LoginAsync([FromBody] LoginRequest request)
        {
            var result = await CarOwnerService.LoginAsync(db, request.Username, request.Password);
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
                        Succeed = false,
                        Message = "用户名或密码为空",
                    };
                case LoginOrRegisterResultType.Wrong:
                    return new ResponseData<LoginResult>()
                    {
                        Succeed = false,
                        Message = "用户名或密码错误",
                    };
                default:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public async Task<ResponseData<LoginResult>> RegisterAsync([FromBody] LoginRequest request)
        {
            var result = await CarOwnerService.RegisterAsync(db, request.Username, request.Password);
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
                        Succeed = false,
                        Message = "用户名已存在",
                    };
                default:
                    throw new NotImplementedException();
            }
        }   /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Password")]
        public async Task<ResponseData<object>> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<object>() { Succeed = false, Message = "用户验证失败" };
            }
            CarOwner carOwner =await db.CarOwners.FindAsync(request.UserID);
            if(carOwner==null)
            {
                return new ResponseData<object>(null, false, "找不到用户");
            } 
            if(carOwner.Password!=CarOwnerService.CreateMD5(carOwner.Username+request.OldPassword))
            {
                return new ResponseData<object>(null, false, "旧密码错误");
            }
            await CarOwnerService.SetPasswordAsync(db, carOwner, request.NewPassword);
                return new ResponseData<object>();
        }

    }

    /// <summary>
    /// 登录请求
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
    }  
    public class ChangePasswordRequest:UserToken
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }
    }

    public class LoginResult : UserToken
    {
        public LoginResult(CarOwner carOwner) : base(carOwner.ID, true)
        {
            CarOwner = carOwner;
            carOwner.Password = null;
        }

        public CarOwner CarOwner { get; set; }
    }
 
}
