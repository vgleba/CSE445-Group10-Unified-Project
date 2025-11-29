namespace Analytics.Utils;

public static class GeoMath
{
    // Haversine distance (km)
    public static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    // Move from (lat,lon) by distance km and bearing deg → new (lat,lon)
    public static (double lat, double lon) Move(double lat, double lon, double distanceKm, double bearingDeg)
    {
        const double R = 6371.0;
        double br = ToRad(bearingDeg);
        double d = distanceKm / R;
        double lat1 = ToRad(lat);
        double lon1 = ToRad(lon);

        double lat2 = Math.Asin(Math.Sin(lat1) * Math.Sin(d) + Math.Cos(lat1) * Math.Cos(d) * Math.Cos(br));
        double lon2 = lon1 + Math.Atan2(
            Math.Sin(br) * Math.Cos(lat1) * Math.Sin(d),
            Math.Cos(d) - Math.Sin(lat1) * Math.Sin(lat2));
        return (ToDeg(lat2), ToDeg(lon2));
    }

    private static double ToRad(double d) => d * Math.PI / 180.0;
    private static double ToDeg(double r) => r * 180.0 / Math.PI;
}
