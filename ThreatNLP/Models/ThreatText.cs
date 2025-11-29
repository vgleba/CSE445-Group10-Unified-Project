namespace Analytics.Models;

public sealed record ThreatText(string Text, string? Lang = null, DateTimeOffset? Ts = null);
