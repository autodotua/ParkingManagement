using Microsoft.EntityFrameworkCore;
using Park.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Park.Service
{
    public static class CarOwnerService
    {        /// <summary>
             /// 注册
             /// </summary>
             /// <param name="db"></param>
             /// <param name="username"></param>
             /// <param name="password"></param>
             /// <param name="registTime"></param>
             /// <returns>如果用户名已存在，返回null，否则返回注册后的用户对象</returns>

        public static Task<LoginOrRegisterResult> Register(ParkContext db, string username, string password)
        {
            return Regist(db, username, password, DateTime.Now);
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="registTime">内部测试使用的时间</param>
        /// <returns>如果用户名已存在，返回null，否则返回注册后的用户对象</returns>
        public async static Task<LoginOrRegisterResult> Regist(ParkContext db, string username, string password, DateTime registTime)
        {
            if (await db.CarOwners.AnyAsync(p => p.Username == username))
            {
                return new LoginOrRegisterResult() { Type = LoginOrRegisterResultType.Existed };
            }
            CarOwner carOwner = new CarOwner()
            {
                RegistTime = registTime,
                LastLoginTime=registTime,
                Username = username,
                Password = CreateMD5(password),
                Enabled = true,
            };
            db.Add(carOwner);
            await db.SaveChangesAsync();
            return new LoginOrRegisterResult() { CarOwner = carOwner };
        }
        public async static Task<LoginOrRegisterResult> Login(ParkContext db, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new LoginOrRegisterResult() { Type = LoginOrRegisterResultType.Empty };
                //return new ResponseData<object>() { Succeed = false, Message = "用户名或密码为空" };
            }
            CarOwner carOwner = await db.CarOwners
                .FirstOrDefaultAsync(p => p.Username == username&&p.Password==CreateMD5(password));

            if (carOwner==null)
            {
                return new LoginOrRegisterResult() { Type = LoginOrRegisterResultType.Wrong };
                //return new ResponseData<object>() { Succeed = false, Message = "用户名或密码错误" };
            }
            carOwner.LastLoginTime = DateTime.Now;
            db.Entry(carOwner).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return new LoginOrRegisterResult() { CarOwner = carOwner };
        }
        public async static Task SetPasswordAsync(ParkContext db, CarOwner carOwner, string password)
        {
            carOwner.Password = CreateMD5(carOwner.Username + password);
            db.Entry(carOwner).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
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


    }
    public class LoginOrRegisterResult
    {
        public LoginOrRegisterResultType Type { get; set; } = LoginOrRegisterResultType.Succeed;
        public CarOwner CarOwner { get; set; }
    }
    public enum LoginOrRegisterResultType
    {
        Succeed,//成功
        Empty,//用户名或密码为空
        Wrong,//用户名或密码不正确
        Existed,//用户已存在

    }
}
