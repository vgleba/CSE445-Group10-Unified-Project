using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using ThreatNLPWebApp.Models;
using ThreatNLPWebApp.Utils;

namespace ThreatNLPWebApp.Services
{
    public sealed class GeoResolveService
    {
        private readonly GazetteerService _gaz;

        public GeoResolveService(GazetteerService gaz)
        {
            _gaz = gaz;
        }

        private static readonly Dictionary<string, int> DirMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["nw"] = 315,
            ["northwest"] = 315,
            ["пн-зх"] = 315,
            ["північний захід"] = 315,
            ["ne"] = 45,
            ["northeast"] = 45,
            ["пн-сх"] = 45,
            ["північний схід"] = 45,
            ["sw"] = 225,
            ["southwest"] = 225,
            ["пд-зх"] = 225,
            ["південний захід"] = 225,
            ["se"] = 135,
            ["southeast"] = 135,
            ["пд-сх"] = 135,
            ["південний схід"] = 135,
            ["n"] = 0,
            ["north"] = 0,
            ["пн"] = 0,
            ["північ"] = 0,
            ["e"] = 90,
            ["east"] = 90,
            ["сх"] = 90,
            ["схід"] = 90,
            ["s"] = 180,
            ["south"] = 180,
            ["пд"] = 180,
            ["південь"] = 180,
            ["w"] = 270,
            ["west"] = 270,
            ["зх"] = 270,
            ["захід"] = 270
        };

        private static readonly Regex OfDir = new Regex(@"(?ix)
            (?<dir>пн-зх|пн-сх|пд-зх|пд-сх|пн|сх|пд|зх|
                      north(?:west|east)?|south(?:west|east)?|east|west|north|south|nw|ne|sw|se)
            \s+(?:of|від)\s+(?<place>[A-ZА-ЯІЇЄҐ][\p{L}\-'\s]{1,40})
        ");

        public GeoResp Resolve(GeoReq req)
        {
            var defR = TryGetDouble("GeoResolve.DefaultRadiusKm", 8.0);
            var off = TryGetDouble("GeoResolve.DirectionOffsetKm", 10.0);

            // 1) Directional "X of PLACE"
            var m = OfDir.Match(req.Location_Text ?? string.Empty);
            if (m.Success)
            {
                var dirKey = m.Groups["dir"].Value.Trim().ToLowerInvariant();
                var placeTxt = m.Groups["place"].Value.Trim();
                var place = _gaz.FindBest(placeTxt);
                if (place != null && DirMap.TryGetValue(dirKey, out var deg))
                {
                    var (lat, lon) = GeoMath.Move(place.Lat, place.Lon, off, deg);
                    return new GeoResp(lat, lon, place.R_km > 0 ? place.R_km : defR);
                }
            }

            // 2) Simple place match (no directional phrase)
            var p2 = _gaz.FindBest(req.Location_Text ?? req.Text ?? "");
            if (p2 != null) return new GeoResp(p2.Lat, p2.Lon, p2.R_km > 0 ? p2.R_km : defR);

            // 3) Fall back to origin (if provided as "lat,lon" or place name)
            if (!string.IsNullOrWhiteSpace(req.Origin))
            {
                if (TryParseLatLon(req.Origin, out var la, out var lo))
                    return new GeoResp(la, lo, defR);

                var po = _gaz.FindBest(req.Origin);
                if (po != null) return new GeoResp(po.Lat, po.Lon, po.R_km > 0 ? po.R_km : defR);
            }

            // Unknown → (0,0) with default radius
            return new GeoResp(0, 0, defR);
        }

        private static double TryGetDouble(string key, double def)
        {
            var s = ConfigurationManager.AppSettings[key];
            return double.TryParse(s, out var v) ? v : def;
        }

        private static bool TryParseLatLon(string s, out double lat, out double lon)
        {
            lat = lon = 0;
            var parts = s.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return false;
            return double.TryParse(parts[0], out lat) && double.TryParse(parts[1], out lon);
        }
    }
}
