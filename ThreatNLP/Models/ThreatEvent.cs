namespace Analytics.Models;

public sealed class ThreatEvent
{
    public string ThreatType { get; set; } = "unknown";    // missile|uav|artillery|unknown
    public int? DirectionDeg { get; set; }                 // 0..359 (N=0, E=90, ...)
    public string? LocationText { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public double Confidence { get; set; } = 0.5;
}
