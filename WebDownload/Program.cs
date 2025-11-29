using WebDownload.Models;
using WebDownload.Options;
using WebDownload.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Config & DI
builder.Services.Configure<DownloadOptions>(builder.Configuration.GetSection("Download"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<DownloadService>((sp, http) => {
    var opt = sp.GetRequiredService<IOptions<DownloadOptions>>().Value;
    http.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
});

// For cross-origin TryIt pages (you can tighten later)
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

// GET /api/download?url=...&raw=false
app.MapGet("/api/download",
    async Task<IResult> (string url, bool? raw, DownloadService svc, CancellationToken ct) =>
    {
        var (code, err, data) = await svc.FetchAsync(new DownloadRequest(url, raw ?? false), ct);
        if (code != 200)
        {
            return Results.Problem(title: "download_failed", detail: err, statusCode: code);
        }
        return Results.Text(data!.Text, "text/plain; charset=utf-8");
    })
.WithName("WebDownload")
.WithSummary("Downloads a page and returns printable text by default.")
.WithDescription("Query: url (required), raw=true to return raw HTML.")
.Produces<string>(StatusCodes.Status200OK, contentType: "text/plain")
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status413PayloadTooLarge)
.Produces(StatusCodes.Status422UnprocessableEntity)
.Produces(StatusCodes.Status502BadGateway)
.Produces(StatusCodes.Status504GatewayTimeout);

app.Run();
