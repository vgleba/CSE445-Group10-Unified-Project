namespace Analytics.Models;

public sealed record GeoReq(string Location_Text, string? Origin = null, string? Text = null);
public sealed record GeoResp(double Lat, double Lon, double R_km);

public sealed class GazetteerPlace
{
    public string Name { get; set; } = "";
    public string[] Aliases { get; set; } = Array.Empty<string>();
    public double Lat { get; set; }
    public double Lon { get; set; }
    public double R_km { get; set; }   // default radius for this place
}
