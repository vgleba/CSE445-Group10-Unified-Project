using WebDownload.Models;
using WebDownload.Options;
using WebDownload.Utils;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace WebDownload.Services;

public sealed class DownloadService
{
    private readonly HttpClient _http;
    private readonly DownloadOptions _opt;

    public DownloadService(HttpClient http, IOptions<DownloadOptions> opt)
    {
        _http = http;
        _opt = opt.Value;
    }

    public async Task<(int StatusCode, string? Error, DownloadResult? Data)> FetchAsync(
        DownloadRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Url))
            return (400, "url is required", null);

        if (!Uri.TryCreate(req.Url, UriKind.Absolute, out var uri))
            return (400, "invalid url", null);

        if (!_opt.AllowedSchemes.Contains(uri.Scheme, StringComparer.OrdinalIgnoreCase))
            return (422, "unsupported scheme", null);

        if (_opt.BlockPrivateNetworks && await IsPrivateAsync(uri.Host).ConfigureAwait(false))
            return (403, "target host is not allowed", null);

        using var message = new HttpRequestMessage(HttpMethod.Get, uri);
        message.Headers.UserAgent.ParseAdd("AAI-WebDownload/1.0");
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.1));

        try
        {
            using var resp = await _http.SendAsync(
                message, HttpCompletionOption.ResponseHeadersRead, ct);

            if (!resp.IsSuccessStatusCode)
                return ((int)resp.StatusCode, $"upstream {(int)resp.StatusCode}", null);

            var contentType = resp.Content.Headers.ContentType?.MediaType ?? "text/plain";
            var charset = resp.Content.Headers.ContentType?.CharSet;

            // enforce content-length if provided
            var len = resp.Content.Headers.ContentLength;
            if (len.HasValue && len.Value > _opt.MaxBytes)
                return (413, "content too large", null);

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var ms = new MemoryStream();
            var buffer = new byte[8192];
            long total = 0;
            int read;
            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
            {
                total += read;
                if (total > _opt.MaxBytes) return (413, "content too large", null);
                ms.Write(buffer, 0, read);
            }

            var bytes = ms.ToArray();
            var encoding = GetEncoding(charset) ?? Encoding.UTF8;
            var body = encoding.GetString(bytes);

            var returnPlain = _opt.ReturnPlainTextByDefault && !req.Raw;
            var text = returnPlain && contentType.Contains("html", StringComparison.OrdinalIgnoreCase)
                ? HtmlToText.Convert(body)
                : body;

            return (200, null, new DownloadResult
            {
                Text = text,
                Charset = encoding.WebName,
                Truncated = false
            });
        }
        catch (TaskCanceledException)
        {
            return (504, "timeout", null);
        }
        catch (HttpRequestException ex)
        {
            return (502, ex.Message, null);
        }
    }

    private static Encoding? GetEncoding(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        try { return Encoding.GetEncoding(name); } catch { return null; }
    }

    // Basic SSRF guard: block localhost and private ranges.
    private static async Task<bool> IsPrivateAsync(string host)
    {
        try
        {
            var addrs = await Dns.GetHostAddressesAsync(host);
            foreach (var ip in addrs)
            {
                if (IPAddress.IsLoopback(ip)) return true;
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    var b = ip.GetAddressBytes();
                    if (b[0] == 10) return true;                               // 10.0.0.0/8
                    if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;  // 172.16/12
                    if (b[0] == 192 && b[1] == 168) return true;               // 192.168/16
                    if (b[0] == 169 && b[1] == 254) return true;               // 169.254/16
                }
                else
                {
                    // IPv6: ::1, fc00::/7 (ULA), fe80::/10 (link-local)
                    if (ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal) return true;
                    if (ip.Equals(IPAddress.IPv6Loopback)) return true;
                }
            }
            return false;
        }
        catch { return true; } // fail closed
    }
}
