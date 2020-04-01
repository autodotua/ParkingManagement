using Park.Core.Models;

namespace Park.Core.Helper
{
    public class LeaveResult
    {
        public bool CanLeave { get; set; }
        public double NeedToPay { get; set; }
        public ParkRecord ParkRecord { get; set; }
    }
}
