namespace WebDownload.Options;

public sealed class DownloadOptions
{
    public int TimeoutSeconds { get; set; } = 10;
    public long MaxBytes { get; set; } = 1_048_576;
    public string[] AllowedSchemes { get; set; } = new[] { "http", "https" };
    public bool BlockPrivateNetworks { get; set; } = true;
    public bool ReturnPlainTextByDefault { get; set; } = true;
}
