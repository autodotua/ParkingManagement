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

namespace Park.API.Controllers
{
    [ApiController]
    [EnableCors("cors")]
    [Route("[controller]")]
    public class ParkController : ControllerBase
    {

        [HttpGet]
        [Route("ohh")]
        public async Task<IEnumerable<object>> Ohh()
        {
            Context db = new Context();
            return await db.CarOwners.ToListAsync();
        }

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
