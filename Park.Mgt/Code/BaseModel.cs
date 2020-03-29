using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using FineUICore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Configuration;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Filters;
using Park.Mgt.Models;
using System.Reflection;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Park.Mgt
{
    public class BaseModel : PageModel
    {
        #region ViewBag

        private DynamicViewData _viewBag;

        /// <summary>
        /// Add ViewBag to PageModel
        /// https://forums.asp.net/t/2128012.aspx?Razor+Pages+ViewBag+has+gone+
        /// https://github.com/aspnet/Mvc/issues/6754
        /// </summary>
        public dynamic ViewBag
        {
            get
            {
                if (_viewBag == null)
                {
                    _viewBag = new DynamicViewData(ViewData);
                }
                return _viewBag;
            }
        }
        #endregion

        #region 只读静态变量

        private static readonly string SK_ONLINE_UPDATE_TIME = "OnlineUpdateTime";

        public static readonly string CHECK_POWER_FAIL_PAGE_MESSAGE = "您无权访问此页面！";
        public static readonly string CHECK_POWER_FAIL_ACTION_MESSAGE = "您无权进行此操作！";

        #endregion

        #region OnActionExecuting

        /// <summary>
        /// 页面处理器调用之前执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            // 如果用户已经登录，更新在线记录
            if (User.Identity.IsAuthenticated)
            {
                UpdateOnlineUser(GetIdentityID());
            }
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            base.OnPageHandlerExecuted(context);
        }

        #endregion

        #region ShowNotify

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        public virtual void ShowNotify(string message)
        {
            ShowNotify(message, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon)
        {
            ShowNotify(message, messageIcon, Target.Top);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        /// <param name="target"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon, Target target)
        {
            Notify n = new Notify
            {
                Target = target,
                Message = message,
                MessageBoxIcon = messageIcon,
                PositionX = Position.Center,
                PositionY = Position.Top,
                DisplayMilliseconds = 3000,
                ShowHeader = false
            };

            n.Show();
        }


        #endregion

        #region 在线用户相关

        protected void UpdateOnlineUser(int? userID)
        {
            if (userID == null)
            {
                return;
            }

            DateTime now = DateTime.Now;
            object lastUpdateTime = HttpContext.Session.GetObject<DateTime>(SK_ONLINE_UPDATE_TIME);
            if (lastUpdateTime == null || (Convert.ToDateTime(lastUpdateTime).Subtract(now).TotalMinutes > 5))
            {
                // 记录本次更新时间
                HttpContext.Session.SetObject<DateTime>(SK_ONLINE_UPDATE_TIME, now);

                Online online = DB.Onlines.Where(o => o.User.ID == userID).FirstOrDefault();
                if (online != null)
                {
                    online.UpdateTime = now;
                    DB.SaveChanges();
                }

            }
        }

        protected async Task RegisterOnlineUserAsync(int userID)
        {
            DateTime now = DateTime.Now;

            Online online = await DB.Onlines.Where(o => o.User.ID == userID).FirstOrDefaultAsync();

            // 如果不存在，就创建一条新的记录
            if (online == null)
            {
                online = new Online();
                DB.Onlines.Add(online);
            }
            online.UserID = userID;
            online.IPAdddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            online.LoginTime = now;
            online.UpdateTime = now;

            await DB.SaveChangesAsync();

            // 记录本次更新时间
            HttpContext.Session.SetObject<DateTime>(SK_ONLINE_UPDATE_TIME, now);
        }

        /// <summary>
        /// 在线人数
        /// </summary>
        /// <returns></returns>
        protected async Task<int> GetOnlineCountAsync()
        {
            DateTime lastM = DateTime.Now.AddMinutes(-15);
            return await DB.Onlines.Where(o => o.UpdateTime > lastM).CountAsync();
        }

        #endregion

        #region 当前登录用户信息

        // http://blog.163.com/zjlovety@126/blog/static/224186242010070024282/
        // http://www.cnblogs.com/gaoshuai/articles/1863231.html
        /// <summary>
        /// 当前登录用户的角色列表
        /// </summary>
        /// <returns></returns>
        protected List<int> GetIdentityRoleIDs()
        {
            return GetIdentityRoleIDs(HttpContext);
        }

        #endregion

        #region 权限检查

        /// <summary>
        /// 检查当前用户是否拥有某个权限
        /// </summary>
        /// <param name="powerType"></param>
        /// <returns></returns>
        protected bool CheckPower(string powerName)
        {
            return CheckPower(HttpContext, powerName);
        }

        /// <summary>
        /// 获取当前登录用户拥有的全部权限列表
        /// </summary>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        protected List<string> GetRolePowerNames()
        {
            return GetRolePowerNames(HttpContext);
        }

        /// <summary>
        /// 检查权限失败（页面第一次加载）
        /// </summary>
        public static void CheckPowerFailWithPage(HttpContext context)
        {
            string PageTemplate = "<!DOCTYPE html><html><head><meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\"/><head><body>{0}</body></html>";
            context.Response.WriteAsync(String.Format(PageTemplate, CHECK_POWER_FAIL_PAGE_MESSAGE));
        }

        /// <summary>
        /// 检查权限失败（页面回发）
        /// </summary>
        public static void CheckPowerFailWithAlert()
        {
            FineUICore.PageContext.RegisterStartupScript(Alert.GetShowInTopReference(CHECK_POWER_FAIL_ACTION_MESSAGE));
        }

        /// <summary>
        /// 检查当前用户是否拥有某个权限
        /// </summary>
        /// <param name="context"></param>
        /// <param name="powerName"></param>
        /// <returns></returns>
        public static bool CheckPower(HttpContext context, string powerName)
        {
            // 如果权限名为空，则放行
            if (String.IsNullOrEmpty(powerName))
            {
                return true;
            }

            // 当前登陆用户的权限列表
            List<string> rolePowerNames = GetRolePowerNames(context);
            if (rolePowerNames.Contains(powerName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取当前登录用户拥有的全部权限列表
        /// </summary>
        /// <param name="roleIDs"></param>
        /// <returns></returns>
        public static List<string> GetRolePowerNames(HttpContext context)
        {
            var db = GetDbConnection();

            // 将用户拥有的权限列表保存在Session中，这样就避免每个请求多次查询数据库
            if (context.Session.GetObject<List<string>>("UserPowerList") == null)
            {
                List<string> rolePowerNames = new List<string>();

                // 超级管理员拥有所有权限
                if (GetIdentityName(context) == "admin")
                {
                    rolePowerNames = db.Powers.Select(p => p.Name).ToList();
                }
                else
                {
                    List<int> roleIDs = GetIdentityRoleIDs(context);

                    var roles = db.Roles
                        .Include(r => r.RolePowers)
                        .ThenInclude(rp => rp.Power)
                        .Where(r => roleIDs.Contains(r.ID))
                        .ToList();
                    foreach (var role in roles)
                    {
                        foreach (var rolepower in role.RolePowers)
                        {
                            if (!rolePowerNames.Contains(rolepower.Power.Name))
                            {
                                rolePowerNames.Add(rolepower.Power.Name);
                            }
                        }
                    }
                }

                context.Session.SetObject<List<string>>("UserPowerList", rolePowerNames);
            }


            return context.Session.GetObject<List<string>>("UserPowerList");
        }

        // http://blog.163.com/zjlovety@126/blog/static/224186242010070024282/
        // http://www.cnblogs.com/gaoshuai/articles/1863231.html
        /// <summary>
        /// 当前登录用户的角色列表
        /// </summary>
        /// <returns></returns>
        public static List<int> GetIdentityRoleIDs(HttpContext context)
        {
            List<int> roleIDs = new List<int>();

            if (context.User.Identity.IsAuthenticated)
            {
                string userData = context.User.Claims.Where(x => x.Type == "RoleIDs").FirstOrDefault().Value;

                foreach (string roleID in userData.Split(','))
                {
                    if (!String.IsNullOrEmpty(roleID))
                    {
                        roleIDs.Add(Convert.ToInt32(roleID));
                    }
                }
            }

            return roleIDs;
        }


        /// <summary>
        /// 当前登录用户名
        /// </summary>
        /// <returns></returns>
        protected string GetIdentityName()
        {
            return GetIdentityName(HttpContext);
        }

        /// <summary>
        /// 当前登录用户名
        /// </summary>
        /// <returns></returns>
        public static string GetIdentityName(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var userName = context.User.Claims.Where(x => x.Type == "UserName").FirstOrDefault().Value;
            return userName;
        }

        /// <summary>
        /// 当前登录用户标识符
        /// </summary>
        /// <returns></returns>
        protected int? GetIdentityID()
        {
            return GetIdentityID(HttpContext);
        }

        /// <summary>
        /// 当前登录用户标识符
        /// </summary>
        /// <returns></returns>
        public static int? GetIdentityID(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var userID = context.User.Claims.Where(x => x.Type == "UserID").FirstOrDefault().Value;
            return Convert.ToInt32(userID);
        }

        #endregion

        #region 模拟树的下拉列表

        protected List<T> ResolveDDL<T>(List<T> mys) where T : ICustomTree, ICloneable, IKeyID, new()
        {
            return ResolveDDL<T>(mys, -1, true);
        }

        protected List<T> ResolveDDL<T>(List<T> mys, int currentId) where T : ICustomTree, ICloneable, IKeyID, new()
        {
            return ResolveDDL<T>(mys, currentId, true);
        }


        // 将一个树型结构放在一个下列列表中可供选择
        protected List<T> ResolveDDL<T>(List<T> source, int currentID, bool addRootNode) where T : ICustomTree, ICloneable, IKeyID, new()
        {
            List<T> result = new List<T>();

            if (addRootNode)
            {
                // 添加根节点
                T root = new T
                {
                    Name = "--根节点--",
                    ID = -1,
                    TreeLevel = 0,
                    Enabled = true
                };
                result.Add(root);
            }

            foreach (T item in source)
            {
                T newT = (T)item.Clone();
                result.Add(newT);

                // 所有节点的TreeLevel加一
                if (addRootNode)
                {
                    newT.TreeLevel++;
                }
            }

            // currentId==-1表示当前节点不存在
            if (currentID != -1)
            {
                // 本节点不可点击（也就是说当前节点不可能是当前节点的父节点）
                // 并且本节点的所有子节点也不可点击，你想如果当前节点跑到子节点的子节点，那么这些子节点就从树上消失了
                bool startChileNode = false;
                int startTreeLevel = 0;
                foreach (T my in result)
                {
                    if (my.ID == currentID)
                    {
                        startTreeLevel = my.TreeLevel;
                        my.Enabled = false;
                        startChileNode = true;
                    }
                    else
                    {
                        if (startChileNode)
                        {
                            if (my.TreeLevel > startTreeLevel)
                            {
                                my.Enabled = false;
                            }
                            else
                            {
                                startChileNode = false;
                            }
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region InvalidModelState

        //protected void InvalidModelState(ModelStateDictionary state)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    sb.Append("<ul>");
        //    foreach (var key in state.Keys)
        //    {
        //        //将错误描述添加到sb中
        //        foreach (var error in state[key].Errors)
        //        {
        //            sb.AppendFormat("<li>{0}</li>", error.ErrorMessage);
        //        }
        //    }
        //    sb.Append("</ul>");

        //    Alert.Show(sb.ToString());
        //}

        #endregion

        #region GetProductVersion

        protected string GetProductVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Build);
        }

        #endregion

        #region Dapper

        private ParkMgtContext _db;
        /// <summary>
        /// 每个请求共享一个数据库连接实例
        /// </summary>
        protected ParkMgtContext DB
        {
            get
            {
                if (_db == null)
                {
                    _db = BaseModel.GetDbConnection();
                }
                return _db;
            }
        }

        /// <summary>
        /// 获取数据库连接实例（静态方法）
        /// </summary>
        /// <returns></returns>
        public static ParkMgtContext GetDbConnection()
        {
            return FineUICore.PageContext.GetRequestService<ParkMgtContext>();
        }


        /// <summary>
        /// 获取实例的属性名称列表
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private string[] GetReflectionProperties(object instance)
        {
            var result = new List<string>();
            foreach (PropertyInfo property in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var propertyName = property.Name;
                // NotMapped特性
                var notMappedAttr = property.GetCustomAttribute<NotMappedAttribute>(false);
                if (notMappedAttr == null && propertyName != "ID")
                {
                    result.Add(propertyName);
                }
            }
            return result.ToArray();
        }


        protected IQueryable<T> Sort<T>(IQueryable<T> q, PagingInfoViewModel pagingInfo)
        {
            return q.SortBy(pagingInfo.SortField + " " + pagingInfo.SortDirection);
        }

        // 排序
        protected IQueryable<T> Sort<T>(IQueryable<T> q, string sortField, string sortDirection)
        {
            return q.SortBy(sortField + " " + sortDirection);
        }


        protected IQueryable<T> SortAndPage<T>(IQueryable<T> q, PagingInfoViewModel pagingInfo)
        {
            return SortAndPage(q, pagingInfo.PageIndex, pagingInfo.PageSize, pagingInfo.RecordCount, pagingInfo.SortField, pagingInfo.SortDirection);
        }

        // 排序后分页
        protected IQueryable<T> SortAndPage<T>(IQueryable<T> q, int pageIndex, int pageSize, int recordCount, string sortField, string sortDirection)
        {
            //// 对传入的 pageIndex 进行有效性验证//////////////
            int pageCount = recordCount / pageSize;
            if (recordCount % pageSize != 0)
            {
                pageCount++;
            }
            if (pageIndex > pageCount - 1)
            {
                pageIndex = pageCount - 1;
            }
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            ///////////////////////////////////////////////

            return Sort(q, sortField, sortDirection).Skip(pageIndex * pageSize).Take(pageSize);
        }


        // 附加实体到数据库上下文中（首先在Local中查找实体是否存在，不存在才Attach，否则会报错）
        // http://patrickdesjardins.com/blog/entity-framework-4-3-an-object-with-the-same-key-already-exists-in-the-objectstatemanager
        protected T Attach<T>(int keyID) where T : class, IKeyID, new()
        {
            T t = DB.Set<T>().Local.Where(x => x.ID == keyID).FirstOrDefault();
            if (t == null)
            {
                t = new T { ID = keyID };
                DB.Set<T>().Attach(t);
            }
            return t;
        }

        //// 向现有实体集合中添加新项
        //protected void AddEntities<T>(ICollection<T> existItems, int[] newItemIDs) where T : class, IKeyID, new()
        //{
        //    foreach (int roleID in newItemIDs)
        //    {
        //        T t = Attach<T>(roleID);
        //        existItems.Add(t);
        //    }
        //}

        //// 替换现有实体集合中的所有项
        //// http://stackoverflow.com/questions/2789113/entity-framework-update-entity-along-with-child-entities-add-update-as-necessar
        //protected void ReplaceEntities<T>(ICollection<T> existEntities, int[] newEntityIDs) where T : class, IKeyID, new()
        //{
        //    if (newEntityIDs.Length == 0)
        //    {
        //        existEntities.Clear();
        //    }
        //    else
        //    {
        //        int[] tobeAdded = newEntityIDs.Except(existEntities.Select(x => x.ID)).ToArray();
        //        int[] tobeRemoved = existEntities.Select(x => x.ID).Except(newEntityIDs).ToArray();

        //        AddEntities<T>(existEntities, tobeAdded);

        //        existEntities.Where(x => tobeRemoved.Contains(x.ID)).ToList().ForEach(e => existEntities.Remove(e));
        //    }
        //}

        //// http://patrickdesjardins.com/blog/validation-failed-for-one-or-more-entities-see-entityvalidationerrors-property-for-more-details-2
        //// ((System.Data.Entity.Validation.DbEntityValidationException)$exception).EntityValidationErrors





        protected T Attach2<T>(int keyID1, int keyID2) where T : class, IKey2ID, new()
        {
            T t = DB.Set<T>().Local.Where(x => x.ID1 == keyID1 && x.ID2 == keyID2).FirstOrDefault();
            if (t == null)
            {
                t = new T { ID1 = keyID1, ID2 = keyID2 };
                DB.Set<T>().Attach(t);
            }
            return t;
        }

        protected void AddEntities2<T>(int keyID1, int[] keyID2s) where T : class, IKey2ID, new()
        {
            foreach (int id in keyID2s)
            {
                T t = Attach2<T>(keyID1, id);
                DB.Entry(t).State = EntityState.Added;

                //T t = new T { ID1 = keyID1, ID2 = id };
                //existEntities.Add(t);
            }
        }

        protected void AddEntities2<T>(int[] keyID1s, int keyID2) where T : class, IKey2ID, new()
        {
            foreach (int id in keyID1s)
            {
                T t = Attach2<T>(id, keyID2);
                DB.Entry(t).State = EntityState.Added;

                //T t = new T { ID1 = id, ID2 = keyID2 };
                //existEntities.Add(t);
            }
        }

        protected void RemoveEntities2<T>(List<T> existEntities, int[] keyID1s, int[] keyID2s) where T : class, IKey2ID, new()
        {
            List<T> itemsTobeRemoved;
            if (keyID1s == null)
            {
                itemsTobeRemoved = existEntities.Where(x => keyID2s.Contains(x.ID2)).ToList();
            }
            else
            {
                itemsTobeRemoved = existEntities.Where(x => keyID1s.Contains(x.ID1)).ToList();
            }
            itemsTobeRemoved.ForEach(e => existEntities.Remove(e));
        }

        protected void ReplaceEntities2<T>(List<T> existEntities, int keyID1, int[] keyID2s) where T : class, IKey2ID, new()
        {
            if (keyID2s.Length == 0)
            {
                existEntities.Clear();
            }
            else
            {
                int[] tobeAdded = keyID2s.Except(existEntities.Select(x => x.ID2)).ToArray();
                int[] tobeRemoved = existEntities.Select(x => x.ID2).Except(keyID2s).ToArray();

                AddEntities2<T>(keyID1, tobeAdded);
                RemoveEntities2<T>(existEntities, null, tobeRemoved);
            }
        }

        protected void ReplaceEntities2<T>(List<T> existEntities, int[] keyID1s, int keyID2) where T : class, IKey2ID, new()
        {
            if (keyID1s.Length == 0)
            {
                existEntities.Clear();
            }
            else
            {
                int[] tobeAdded = keyID1s.Except(existEntities.Select(x => x.ID1)).ToArray();
                int[] tobeRemoved = existEntities.Select(x => x.ID1).Except(keyID1s).ToArray();

                AddEntities2<T>(tobeAdded, keyID2);
                RemoveEntities2<T>(existEntities, tobeRemoved, null);
            }
        }

        #endregion


    }
}
