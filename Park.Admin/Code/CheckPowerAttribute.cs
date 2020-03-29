using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Park.Admin
{
    /// <summary>
    /// Park.Admin自定义权限验证过滤器
    /// </summary>
    public class CheckPowerAttribute : ResultFilterAttribute
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            HttpContext context = filterContext.HttpContext;
            // 权限验证不通过
            if (!String.IsNullOrEmpty(Name) && !BaseModel.CheckPower(context, Name))
            {
                if (context.Request.Method == "GET")
                {
                    BaseModel.CheckPowerFailWithPage(context);

                    //http://stackoverflow.com/questions/9837180/how-to-skip-action-execution-from-an-actionfilter
                    // -修正越权访问页面时会报错[服务器无法在发送 HTTP 标头之后追加标头]（龙涛软件-9374）。
                    filterContext.Result = new EmptyResult();
                }
                else if (context.Request.Method == "POST")
                {
                    BaseModel.CheckPowerFailWithAlert();
                    filterContext.Result = UIHelper.Result();
                }
            }

        }




    }
}