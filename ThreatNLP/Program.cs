using Analytics.Models;
using Analytics.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// services
builder.Services.AddSingleton<GazetteerService>();
builder.Services.AddSingleton<ThreatNlpService>();
builder.Services.AddSingleton<GeoResolveService>();

// CORS (if you’ll call from a different origin TryIt)
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

// serve TryIt later
app.UseDefaultFiles();
app.UseStaticFiles();

// ThreatNLP
app.MapPost("/api/threat/nlp", (ThreatText dto, ThreatNlpService svc) =>
{
    var events = svc.Parse(dto.Text ?? string.Empty, dto.Lang, dto.Ts);
    return Results.Json(events);
})
.WithSummary("Parses an alert text into structured threat events.");

// GeoResolve
app.MapPost("/api/threat/georesolve", (GeoReq req, GeoResolveService svc) =>
{
    var resp = svc.Resolve(req);
    return Results.Json(resp);
})
.WithSummary("Resolves a place/directional phrase to coordinates + radius.");

app.Run();
