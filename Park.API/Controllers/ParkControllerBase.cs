using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace Park.API.Controllers
{
    [ApiController]
    [EnableCors("cors")]
    [Route("[controller]")]
    public abstract class ParkControllerBase : ControllerBase
    {
        protected ParkControllerBase()
        {
            db = new Context();
        }
        protected Context db { get; private set; }

    }
}
