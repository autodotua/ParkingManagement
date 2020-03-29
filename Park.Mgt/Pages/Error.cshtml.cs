using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Park.Mgt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Park.Mgt.Pages
{
    public class ErrorModel : BaseModel
    {
        public void OnGet()
        {
            ViewBag.ErrorMessage = "Error";

            var exception = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            if (exception != null)
            {
                ViewBag.ErrorMessage = exception.Error.Message;
            }

            ViewBag.RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        }
    }
}