using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;

using Park.Mgt.Models;
using Microsoft.EntityFrameworkCore;

namespace Park.Mgt.Models
{
    public class MenuHelper
    {
        private static List<Menu> _menus;

        public static List<Menu> Menus
        {
            get
            {
                if (_menus == null)
                {
                    InitMenus();
                }
                return _menus;
            }
        }

        public static void Reload()
        {
            _menus = null;
        }

        private static void InitMenus()
        {
            _menus = new List<Menu>();

            var db = BaseModel.GetDbConnection();

            List<Menu> dbMenus = db.Menus.Include(m => m.ViewPower).OrderBy(m => m.SortIndex).ToList();

            ResolveMenuCollection(dbMenus, null, 0);

        }


        private static int ResolveMenuCollection(List<Menu> dbMenus, Menu parentMenu, int level)
        {
            int count = 0;

            foreach (var menu in dbMenus.Where(m => m.Parent == parentMenu))
            {
                count++;

                _menus.Add(menu);
                menu.TreeLevel = level;
                menu.IsTreeLeaf = true;
                menu.Enabled = true;

                level++;
                int childCount = ResolveMenuCollection(dbMenus, menu, level);
                if (childCount != 0)
                {
                    menu.IsTreeLeaf = false;
                }
                level--;
            }

            return count;
        }

    }
}
