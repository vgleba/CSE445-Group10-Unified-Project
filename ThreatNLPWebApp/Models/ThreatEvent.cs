using System;

namespace ThreatNLPWebApp.Models
{
    public class ThreatEvent
    {
        public string ThreatType { get; set; } = "unknown";
        public int? DirectionDeg { get; set; }
        public string LocationText { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public double Confidence { get; set; } = 0.5;
    }
}