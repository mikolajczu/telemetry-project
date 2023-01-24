using Telemetry.Entities.Models;

namespace Telemetry.ViewModels;

public class TelemetrySessionViewModel
{
    public string Id { get; set; }
    public UserViewModel User { get; set; }
    public ICollection<PageViewModel> Pages { get; set; }
    public DateTime SessionDate { get; set; }
    public double Time { get; set; }
    public byte Status { get; set; } // 0 - OFF, 1 - ON
}