using System;
using System.Collections.Generic;
using System.Text;

namespace Park.Designer.Model
{
    public abstract class ParkObjectBase
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Class { get; set; }
     
    }
}
