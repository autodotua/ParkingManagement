using Park.Models;

namespace Park.Service
{
    /// <summary>
    /// 离场结果
    /// </summary>
    public class LeaveResult
    {
        /// <summary>
        /// 是否可以放行
        /// </summary>
        public bool CanLeave { get; set; }
        /// <summary>
        /// 需要支付的费用
        /// </summary>
        public double NeedToPay { get; set; }
        /// <summary>
        /// 停车记录
        /// </summary>
        public ParkRecord ParkRecord { get; set; }
    }
    /// <summary>
    /// 进场结果
    /// </summary>
    public class EnterResult
    {
        public EnterResult(bool canEnter, string message="")
        {
            CanEnter = canEnter;
            Message = message;
        }
        /// <summary>
        /// 是否允许进入
        /// </summary>
        public bool CanEnter { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; } = "";
    }
}
