using System;
using System.Collections.Generic;
using System.Web;

using System.Linq;

namespace Park.Admin.Models
{
    public class ConfigHelper
    {
        #region fields & constructor

        private static List<Config> _configs;

        private static List<String> changedKeys = new List<string>();

        public static List<Config> Configs
        {
            get
            {
                if (_configs == null)
                {
                    _configs = BaseModel.GetDbConnection().Configs.ToList();
                }
                return _configs;
            }
        }

        public static void Reload()
        {
            _configs = null;
        }

        #endregion

        #region methods

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return Configs.Where(c => c.ConfigKey == key).Select(c => c.ConfigValue).FirstOrDefault();
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(string key, string value)
        {
            Config config = Configs.Where(c => c.ConfigKey == key).FirstOrDefault();
            if (config != null)
            {
                if (config.ConfigValue != value)
                {
                    changedKeys.Add(key);
                    config.ConfigValue = value;
                }
            }
        }

        /// <summary>
        /// 保存所有更改的配置项
        /// </summary>
        public static void SaveAll()
        {
            var db = BaseModel.GetDbConnection();

            var changedConfigs = db.Configs.Where(c => changedKeys.Contains(c.ConfigKey));
            foreach (var changed in changedConfigs)
            {
                changed.ConfigValue = GetValue(changed.ConfigKey);
            }

            db.SaveChanges();

            Reload();

        }

        #endregion

        #region IsDemo

        /// <summary>
        /// 是否演示模式
        /// </summary>
        public static bool IsDemo
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsDemo"]);
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// 网站标题
        /// </summary>
        public static string Title
        {
            get
            {
                return GetValue("Title");
            }
            set
            {
                SetValue("Title", value);
            }
        }

        /// <summary>
        /// 列表每页显示的个数
        /// </summary>
        public static int PageSize
        {
            get
            {
                return Convert.ToInt32(GetValue("PageSize"));
            }
            set
            {
                SetValue("PageSize", value.ToString());
            }
        }

        /// <summary>
        /// 帮助下拉列表
        /// </summary>
        public static string HelpList
        {
            get
            {
                return GetValue("HelpList");
            }
            set
            {
                SetValue("HelpList", value);
            }
        }


        /// <summary>
        /// 菜单样式
        /// </summary>
        public static string MenuType
        {
            get
            {
                return GetValue("MenuType");
            }
            set
            {
                SetValue("MenuType", value);
            }
        }


        /// <summary>
        /// 网站主题
        /// </summary>
        public static string Theme
        {
            get
            {
                return GetValue("Theme");
            }
            set
            {
                SetValue("Theme", value);
            }
        }


        #endregion
    }
}
