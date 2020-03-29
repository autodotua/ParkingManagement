using Park.Mgt.Models;

using FineUICore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Park.Mgt.Pages
{
    public class LoginModel : BaseModel
    {
        public string Window1Title { get; set; }

        public void OnGet()
        {
            LoadData();

        }

        private void LoadData()
        {
            Window1Title = String.Format("Park.Mgt v{0}", GetProductVersion());

        }
        public async Task<IActionResult> OnPostBtnSubmit_ClickAsync(string tbxUserName, string tbxPassword)
        {
            string userName = tbxUserName.Trim();
            string password = tbxPassword.Trim();

            User user = await DB.Users
                .Include(u => u.RoleUsers)
                .Where(u => u.Name == userName).AsNoTracking().FirstOrDefaultAsync();

            if (user != null)
            {
                if (PasswordUtil.ComparePasswords(user.Password, password))
                {
                    if (!user.Enabled)
                    {
                        Alert.Show("用户未启用，请联系管理员！");
                    }
                    else
                    {
                        // 登录成功
                        await LoginSuccess(user);

                        // 重定向到登陆后首页
                        return RedirectToPage("/Index");
                    }
                }
                else
                {
                    Alert.Show("用户名或密码错误！");
                }
            }
            else
            {
                Alert.Show("用户名或密码错误！");
            }

            return UIHelper.Result();
        }

        private async Task LoginSuccess(User user)
        {
            await RegisterOnlineUserAsync(user.ID);

            // 用户所属的角色字符串，以逗号分隔
            string roleIDs = String.Empty;
            if (user.RoleUsers != null)
            {
                roleIDs = String.Join(",", user.RoleUsers.Select(r => r.RoleID).ToArray());
            }

            var claims = new[] { new Claim("UserID", user.ID.ToString()), new Claim("UserName", user.Name), new Claim("RoleIDs", roleIDs) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal userInfo = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userInfo,
                new AuthenticationProperties()
                {
                    IsPersistent = false
                });

        }
    }
}