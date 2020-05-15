using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Park.Models
{
    public static class DbHelper
    {
        /// <summary>
        /// 获取最后一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Task<T> LastOrDefaultRecordAsync<T>(this IQueryable<T> db, Expression<Func<T, DateTime>> key, Expression<Func<T, bool>> predicate = null) where T : class
        {
            var ordered = db.OrderByDescending(key);
            if (predicate == null)
            {
                return ordered.FirstOrDefaultAsync();
            }
            else
            {
                return ordered.FirstOrDefaultAsync(predicate);
            }
        }
    }
}
