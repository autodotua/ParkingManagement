using System;
using System.Collections.Generic;
using System.Text;

namespace Park.Models
{
    public interface IParkObject:IDbModel
    {
        /// <summary>
        /// 分类，备用属性，暂时没用上
        /// </summary>
        public string Class { get; set; }
        public int ParkAreaID { get; set; }
        public ParkArea ParkArea { get; set; }

    }
}
