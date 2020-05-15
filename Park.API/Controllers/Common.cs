using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Park.Models;


namespace Park.API.Controllers
{
    /// <summary>
    /// 通用的服务器返回浏览器的回应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseData<T>
    {
        public ResponseData(T data)
        {
            Data = data;
        }
        public ResponseData()
        {
        }
        /// <summary>
        /// 执行是否成功
        /// </summary>
        public bool Succeed { get; set; } = true;
        /// <summary>
        /// 执行失败时的信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 数据主体
        /// </summary>
        public T Data { get; set; }
    }
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class Context : ParkContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ///从appsettings获取数据库连接字符串
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ParkSQLServer"));
            //base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer(connStr);
        }
    }
    /// <summary>
    /// 用于验证用户信息的类。除了登录和注册，所有的请求都需要携带userID和Token，来验证用户是否合法
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 用户密钥
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 用于生成Token中加密字符串的密钥
        /// </summary>
        private const string Key = "ParkKey";

        public UserToken()
        { }
        public UserToken(int userID, bool createToken)
        {
            UserID = userID;
            if (createToken)
            {
                Token = GetToken();
            }
        }
        /// <summary>
        /// 判断UserID和Token是否匹配，进而判断用户是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            var aes = new FzLib.Cryptography.Aes();
            aes.SetStringKey(Key + UserID);
            aes.SetStringIV("");
            try
            {
                string[] items = aes.Decrypt(Token).Split("-");
                if (items[0] != UserID.ToString())
                {
                    return false;
                }
                //预留过期检测
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            var aes = new FzLib.Cryptography.Aes();
            aes.SetStringKey(Key + UserID);
            aes.SetStringIV("");
            return aes.Encrypt(string.Join("-", UserID.ToString(), DateTime.Now.ToString("yyyyMMdd")));
        }
    }
}
