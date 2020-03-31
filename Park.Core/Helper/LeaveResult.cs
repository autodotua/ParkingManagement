namespace Park.Core.Helper
{
    public class LeaveResult
    {
        public bool CanLeave { get; set; }
        public double NeedToPay { get; set; }
        public readonly static LeaveResult Go = new LeaveResult() { CanLeave = true };
    }
}
