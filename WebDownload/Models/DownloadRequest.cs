namespace WebDownload.Models;

public sealed record DownloadRequest(string Url, bool Raw = false);
