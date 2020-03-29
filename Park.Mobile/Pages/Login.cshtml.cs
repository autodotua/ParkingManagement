using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FineUICore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Park.Mobile.Pages
{
    public class LoginModel : BaseModel
    {
        public void OnGet()
        {

        }

        public IActionResult OnPostBtnLogin_Click(string tbxUserName, string tbxPassword)
        {
            if (tbxUserName == "admin" && tbxPassword == "admin")
            {
                ShowNotify("成功登录！",  MessageBoxIcon.Success);
            }
            else
            {
                ShowNotify("用户名或密码错误！", MessageBoxIcon.Error);
            }

            return UIHelper.Result();
        }

    }
}