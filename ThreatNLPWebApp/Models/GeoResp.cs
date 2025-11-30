namespace ThreatNLPWebApp.Models
{
    public class GeoResp
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double R_km { get; set; }

        public GeoResp(double lat, double lon, double r)
        {
            Lat = lat;
            Lon = lon;
            R_km = r;
        }
    }
}