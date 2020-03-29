using Park.Mgt.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Park.Mgt
{
    [Authorize]
    public class BaseAdminModel : BaseModel
    {

    }
}
