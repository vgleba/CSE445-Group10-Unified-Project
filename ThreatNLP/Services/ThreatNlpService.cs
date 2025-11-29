using Analytics.Models;
using System.Text.RegularExpressions;

namespace Analytics.Services;

public sealed class ThreatNlpService
{
    private static readonly (string type, Regex rx)[] Threats = new[]
    {
        ("uav",       new Regex(@"(?i)\b(бпла|дрон(?:и|ів)?|shahed|шахед(?:и|ів)?|uav|drone(?:s)?)\b")),
        ("missile",   new Regex(@"(?i)\b(ракет(?:а|и|ний)|кинджал|kinzhal|missile(?:s)?|rocket(?:s)?)\b")),
        ("artillery", new Regex(@"(?i)\b(арт(?:обстріл)?|обстріл|mortar(?:s)?|shell(?:ing|s)?)\b")),
    };

    private static readonly Dictionary<string, int> DirMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // UA short
        ["пн"] = 0,
        ["пн-сх"] = 45,
        ["сх"] = 90,
        ["пд-сх"] = 135,
        ["пд"] = 180,
        ["пд-зх"] = 225,
        ["зх"] = 270,
        ["пн-зх"] = 315,
        // UA long
        ["північ"] = 0,
        ["північний схід"] = 45,
        ["схід"] = 90,
        ["південний схід"] = 135,
        ["південь"] = 180,
        ["південний захід"] = 225,
        ["захід"] = 270,
        ["північний захід"] = 315,
        // EN
        ["n"] = 0,
        ["ne"] = 45,
        ["e"] = 90,
        ["se"] = 135,
        ["s"] = 180,
        ["sw"] = 225,
        ["w"] = 270,
        ["nw"] = 315,
        ["north"] = 0,
        ["northeast"] = 45,
        ["east"] = 90,
        ["southeast"] = 135,
        ["south"] = 180,
        ["southwest"] = 225,
        ["west"] = 270,
        ["northwest"] = 315
    };

    private static readonly Regex DirRx = new(@"(?ix)
        \b(
            пн(?:\s*[-–]\s*сх)?|
            пд(?:\s*[-–]\s*сх)?|
            пн(?:\s*[-–]\s*зх)?|
            пд(?:\s*[-–]\s*зх)?|
            північний?\s*захід|північний?\s*схід|південь|південний\s*захід|південний\s*схід|схід|захід|
            north(?:\s*west|\s*east)?|south(?:\s*west|\s*east)?|east|west|north|south|
            n|ne|e|se|s|sw|w|nw
        )\b");

    private static readonly Regex LocRx = new(@"(?ix)
        \b(?:in|near|over|towards|to|at|around|by|within|
             в|у|біля|поблизу|над|в\s+районі|у\s+районі|напрямку|в\s+напрямку)\s+
        (?<loc>[A-ZА-ЯІЇЄҐ][\p{L}\-'\s]{1,40})
    ");

    public ThreatEvent[] Parse(string text, string? langHint = null, DateTimeOffset? ts = null)
    {
        if (string.IsNullOrWhiteSpace(text)) return Array.Empty<ThreatEvent>();
        var t = text.Trim();

        var type = "unknown"; double conf = 0.4;
        foreach (var (canonical, rx) in Threats)
            if (rx.IsMatch(t)) { type = canonical; conf = 0.7; break; }

        int? deg = null;
        var md = DirRx.Match(t);
        if (md.Success)
        {
            var key = md.Groups[1].Value.Trim().ToLowerInvariant().Replace("–", "-");
            key = key.Replace("  ", " ");
            if (DirMap.TryGetValue(key, out var d)) deg = d;
        }

        string? loc = null;
        var ml = LocRx.Match(t);
        if (ml.Success) { loc = ml.Groups["loc"].Value.Trim(); conf += 0.1; }

        var tsOut = ts ?? DateTimeOffset.UtcNow;
        conf = Math.Max(0, Math.Min(1, conf + (deg.HasValue ? 0.1 : 0)));

        return new[] { new ThreatEvent {
            ThreatType = type, DirectionDeg = deg, LocationText = loc, Timestamp = tsOut, Confidence = conf
        }};
    }
}
