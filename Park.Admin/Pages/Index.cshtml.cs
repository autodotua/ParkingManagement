using Park.Admin.Models;
using FineUICore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Park.Admin.Pages
{
    [Authorize]
    public class IndexModel : BaseModel
    {
        public TreeNode[] MenuTreeNodes { get; set; }
        public string UserName { get; set; }
        public string OnlineUserCount { get; set; }
        public string ProductVersion { get; set; }
        public string ConfigTitle { get; set; }
        public FineUICore.Menu SystemHelpMenu { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }


        #region LoadDataAsync

        private async Task LoadDataAsync()
        {
            // 用户可见的菜单列表
            List<Models.Menu> menus = ResolveUserMenuList();
            if (menus.Count == 0)
            {
                ShowNotify("系统管理员尚未给你配置菜单！");
                return;
            }

            MenuTreeNodes = GetTreeNodes(menus).ToArray();

            UserName = GetIdentityName();
            OnlineUserCount = (await GetOnlineCountAsync()).ToString();
            ProductVersion = GetProductVersion();
            ConfigTitle = "停车场管理系统"; // String.Format("Park.Admin v{0}", GetProductVersion());

            SystemHelpMenu = GetSystemHelpMenu();
        }

        // 帮助菜单
        private FineUICore.Menu GetSystemHelpMenu()
        {
            FineUICore.Menu menu = new FineUICore.Menu();

            JArray ja = JArray.Parse(ConfigHelper.HelpList);
            foreach (JObject jo in ja)
            {
                string text = jo.Value<string>("Text");
                Icon icon = IconHelper.String2Icon(jo.Value<string>("Icon"), true);
                string id = jo.Value<string>("ID");
                string url = jo.Value<string>("URL");

                if (!String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(url))
                {
                    FineUICore.MenuButton menuItem = new FineUICore.MenuButton();
                    menuItem.Text = text;
                    menuItem.Icon = icon;
                    menuItem.OnClientClick = String.Format("addExampleTab('{0}','{1}','{2}')", id, Url.Content(url), text);

                    menu.Items.Add(menuItem);
                }
            }

            return menu;
        }

        #endregion

        #region GetTreeNodes

        /// <summary>
        /// 创建树菜单
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private IList<TreeNode> GetTreeNodes(List<Models.Menu> menus)
        {
            IList<TreeNode> nodes = new List<TreeNode>();

            // 生成树
            ResolveMenuTree(menus, null, nodes);

            // 展开第一个树节点
            nodes[0].Expanded = true;

            return nodes;
        }

        /// <summary>
        /// 生成菜单树
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="parentMenuId"></param>
        /// <param name="nodes"></param>
        private int ResolveMenuTree(List<Models.Menu> menus, int? parentMenuID, IList<TreeNode> nodes)
        {
            int count = 0;
            foreach (var menu in menus.Where(m => m.ParentID == parentMenuID))
            {
                TreeNode node = new TreeNode();
                nodes.Add(node);
                count++;

                node.Text = menu.Name;
                node.IconUrl = menu.ImageUrl;
                if (!String.IsNullOrEmpty(menu.NavigateUrl))
                {
                    node.NavigateUrl = Url.Content(menu.NavigateUrl);
                }

                if (menu.IsTreeLeaf)
                {
                    node.Leaf = true;

                    // 如果是叶子节点，但不是超链接，则是空目录，删除
                    if (String.IsNullOrEmpty(menu.NavigateUrl))
                    {
                        nodes.Remove(node);
                        count--;
                    }
                }
                else
                {
                    int childCount = ResolveMenuTree(menus, menu.ID, node.Nodes);

                    // 如果是目录，但是计算的子节点数为0，可能目录里面的都是空目录，则要删除此父目录
                    if (childCount == 0 && String.IsNullOrEmpty(menu.NavigateUrl))
                    {
                        nodes.Remove(node);
                        count--;
                    }
                }

            }

            return count;
        }

        #endregion

        #region ResolveUserMenuList

        // 获取用户可用的菜单列表
        private List<Models.Menu> ResolveUserMenuList()
        {
            // 当前登陆用户的权限列表
            List<string> rolePowerNames = GetRolePowerNames();

            // 当前用户所属角色可用的菜单列表
            List<Models.Menu> menus = new List<Models.Menu>();

            foreach (var menu in MenuHelper.Menus)
            {
                // 如果此菜单不属于任何模块，或者此用户所属角色拥有对此模块的权限
                if (menu.ViewPowerID == null || rolePowerNames.Contains(menu.ViewPower.Name))
                {
                    menus.Add(menu);
                }
            }

            return menus;
        }

        #endregion

        #region btnSignOut_Click
        public async Task<IActionResult> OnPostBtnSignOut_ClickAsync()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }


        #endregion
    }
}
