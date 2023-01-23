namespace Telemetry.ViewModels;

public class TelemetrySessionViewModel
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public UserViewModel User { get; set; }
    public ICollection<UserPageViewModel> Pages { get; set; }
    public DateTime SessionDate { get; set; }
    public double Time { get; set; }
    public byte Status { get; set; } // 0 - OFF, 1 - ON
}