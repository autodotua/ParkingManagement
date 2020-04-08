using Park.Models;

namespace Park.Service
{
    public class LeaveResult
    {
        public bool CanLeave { get; set; }
        public double NeedToPay { get; set; }
        public ParkRecord ParkRecord { get; set; }
    }
}
