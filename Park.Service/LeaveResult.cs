using Park.Models;

namespace Park.Service
{
    public class LeaveResult
    {
        public bool CanLeave { get; set; }
        public double NeedToPay { get; set; }
        public ParkRecord ParkRecord { get; set; }
    }

    public class EnterResult
    {
        public EnterResult(bool canEnter, string message="")
        {
            CanEnter = canEnter;
            Message = message;
        }

        public bool CanEnter { get; set; }
        public string Message { get; set; } = "";
    }
}
