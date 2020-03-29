using Park.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Park.Admin
{
    [Authorize]
    public class BaseAdminModel : BaseModel
    {

    }
}
