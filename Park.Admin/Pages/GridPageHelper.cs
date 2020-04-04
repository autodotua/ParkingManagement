using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Park.Admin.Pages
{
    public class GridPageHelper
    {
        public FineUICore.GridTagHelper GetGrid()
        {
            return new FineUICore.GridTagHelper();
        }
        FineUICore.GridTagHelper gridTagHelper = new FineUICore.GridTagHelper();
    }

}
