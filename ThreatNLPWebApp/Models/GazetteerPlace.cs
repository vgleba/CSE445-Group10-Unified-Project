using System;

namespace ThreatNLPWebApp.Models
{
    public class GazetteerPlace
    {
        public string Name { get; set; } = "";
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double R_km { get; set; }
    }
}