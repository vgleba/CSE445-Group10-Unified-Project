using System;

namespace ThreatNLPWebApp.Models
{
    public class ThreatText
    {
        public string Text { get; set; }
        public string Lang { get; set; }
        public DateTimeOffset? Ts { get; set; }
    }
}