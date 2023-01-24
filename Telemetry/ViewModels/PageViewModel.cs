namespace Telemetry.ViewModels;

public class PageViewModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public double Time
    {
        get => Math.Round(_time, 2, MidpointRounding.AwayFromZero);
        set => _time = value;
    }
    private double _time;
}