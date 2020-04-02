using System;
using System.Collections.Generic;
using System.Text;

namespace Park.Core.Models
{
    public interface IParkObject
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Class { get; set; }
     
    }
}
