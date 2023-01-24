using Telemetry.Entities.Models;

namespace Telemetry.ViewModels;

public class TelemetrySessionViewModel
{
    public string Id { get; set; }
    public UserViewModel User { get; set; }
    public ICollection<PageViewModel> Pages { get; set; }
    public DateTime SessionDate { get; set; }

    private double _time;

    public double Time
    {
        get => Math.Round(_time, 2, MidpointRounding.AwayFromZero);
        set => _time = value;
    }

    public byte Status { get; set; } // 0 - OFF, 1 - ON
}