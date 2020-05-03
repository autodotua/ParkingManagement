using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Park.Models;


namespace Park.API.Controllers
{

    public class ResponseData<T>
    {
        public ResponseData(T data)
        {
            Data = data;
        }
          public ResponseData()
        {
        }

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

}
