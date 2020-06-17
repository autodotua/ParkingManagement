using Microsoft.EntityFrameworkCore;
using Park.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Park.Service
{
    /// <summary>
    /// 车主相关
    /// </summary>
    public static class CarOwnerService
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>如果用户名已存在，返回null，否则返回注册后的用户对象</returns>

        public static Task<LoginOrRegisterResult> RegisterAsync(ParkContext db, string username, string password)
        {
            return RegisterAsync(db, username, password, DateTime.Now);
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="registTime">内部测试使用的时间</param>
        /// <returns>如果用户名已存在，返回null，否则返回注册后的用户对象</returns>
        public async static Task<LoginOrRegisterResult> RegisterAsync(ParkContext db, string username, string password, DateTime registTime)
        {
            if (await db.CarOwners.AnyAsync(p => p.Username == username))
            {
                return new LoginOrRegisterResult() { Type = LoginOrRegisterResultType.Existed };
            }
            CarOwner carOwner = new CarOwner()
            {
                RegistTime = registTime,
                LastLoginTime = registTime,
                Username = username,
                Password = CreateMD5(username+ password),
                Enabled = true,
            };
            db.Add(carOwner);
            await db.SaveChangesAsync();
            return new LoginOrRegisterResult() { CarOwner = carOwner };
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async static Task<LoginOrRegisterResult> LoginAsync(ParkContext db, string username, string password)
        {
            //判断参数是否为空
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new LoginOrRegisterResult() { Type = LoginOrRegisterResultType.Empty };
            }
            if(password.Length!=32)
            {
                password = CreateMD5(username + password);
            }
            //寻找用户名与密码都匹配的用户
            CarOwner carOwner = await db.CarOwners
                .FirstOrDefaultAsync(p => p.Username == username && p.Password == password);


            if (carOwner == null)
            {
                //返回用户名或密码错误
                return new LoginOrRegisterResult() { Type = LoginOrRegisterResultType.Wrong };
            }
            carOwner.LastLoginTime = DateTime.Now;
            db.Entry(carOwner).State = EntityState.Modified;
            //修改并保存用户信息
            await db.SaveChangesAsync();
            return new LoginOrRegisterResult() { CarOwner = carOwner };
        }
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="db"></param>
        /// <param name="carOwner">车主</param>
        /// <param name="password">新密码</param>
        /// <returns></returns>
        public async static Task SetPasswordAsync(ParkContext db, CarOwner carOwner, string password)
        {
            carOwner.Password = CreateMD5(carOwner.Username + password);
            db.Entry(carOwner).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
        /// <summary>
        /// 为密码创建MD5。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
    /// <summary>
    /// 登陆注册结果
    /// </summary>
    public class LoginOrRegisterResult
    {
        /// <summary>
        /// 结果类型
        /// </summary>
        public LoginOrRegisterResultType Type { get; set; } = LoginOrRegisterResultType.Succeed;
        public CarOwner CarOwner { get; set; }
    }
    public enum LoginOrRegisterResultType
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeed,
        /// <summary>
        /// 用户名或密码为空
        /// </summary>
        Empty,
        /// <summary>
        /// 用户名或密码不正确
        /// </summary>
        Wrong,
        /// <summary>
        /// 用户已存在
        /// </summary>
        Existed,

    }
}
