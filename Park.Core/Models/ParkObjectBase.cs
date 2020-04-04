using System;
using System.Collections.Generic;
using System.Text;

namespace Park.Core.Models
{
    public interface IParkObject:IDbModel
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string Class { get; set; }
     
    }
}
