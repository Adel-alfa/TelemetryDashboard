using TelemetryDashboard.Web.Components;
using TelemetryDashboard.Application;
using TelemetryDashboard.Domain.Entities;
using TelemetryDashboard.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ITelemetryStream, RandomTelemetryStream>();

builder.Services.AddHttpClient("TelemetryStream", (sp, client) =>
    {
        var environment = sp.GetRequiredService<IHostEnvironment>();
        bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        if (environment.IsProduction() || isDocker)
        {
            // Use your online published URL when in Production OR inside Docker
            client.BaseAddress = new Uri("https://telemetrydashboard-y37x.onrender.com/");
        }
        else
        {
            // Use local URL (http, https, or IIS) when developing on your machine
            client.BaseAddress = new Uri("http://localhost:5187/");
        }

        client.Timeout = Timeout.InfiniteTimeSpan; // Essential for SSE streams
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapGet("/api/telemetry/stream", (ITelemetryStream telemetryStream, CancellationToken cancellationToken) =>
        TypedResults.ServerSentEvents(telemetryStream.ReadAsync(cancellationToken), "telemetry"))
    .WithName("StreamTelemetry")
    .Produces<TelemetryMetric>(StatusCodes.Status200OK, "text/event-stream");

//app.MapTelemetryStream(); // instead of the above commented-out code, using the extension method defined below

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

/// <summary>Maps the telemetry HTTP streaming endpoint.</summary>
internal static class TelemetryEndpoints
{
    /// <summary>Registers a Server-Sent Events endpoint for remote telemetry consumers.</summary>
    /// <param name="endpoints">The endpoint route builder to add the route to.</param>
    /// <returns>The route handler builder for the registered endpoint.</returns>
    /// <remarks>
    /// The handler adapts the application's in-process <see cref="ITelemetryStream"/> service into an
    /// out-of-process HTTP SSE response with the <c>telemetry</c> event name.
    /// </remarks>
    internal static RouteHandlerBuilder MapTelemetryStream(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/api/telemetry/stream", HandleTelemetryStream)
            .WithName("StreamTelemetry")
            .WithSummary("Stream live telemetry samples")
            .WithDescription("Returns telemetry metrics as Server-Sent Events. The HTTP request cancellation token stops the underlying asynchronous stream.")
            .Produces<TelemetryMetric>(StatusCodes.Status200OK, "text/event-stream");

    /// <summary>Creates the SSE response by enumerating the telemetry service.</summary>
    /// <param name="telemetryStream">The in-process telemetry stream to enumerate.</param>
    /// <param name="cancellationToken">The HTTP request cancellation token propagated to the stream.</param>
    /// <returns>An HTTP result that writes each metric as a <c>telemetry</c> SSE event.</returns>
    private static IResult HandleTelemetryStream(
        ITelemetryStream telemetryStream,
        CancellationToken cancellationToken) =>
        TypedResults.ServerSentEvents(telemetryStream.ReadAsync(cancellationToken), "telemetry");
}
