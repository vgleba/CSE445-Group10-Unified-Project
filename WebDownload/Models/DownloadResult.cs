namespace WebDownload.Models;

public sealed class DownloadResult
{
    public required string Text { get; init; }
    public required string Charset { get; init; }
    public bool Truncated { get; init; }
}
