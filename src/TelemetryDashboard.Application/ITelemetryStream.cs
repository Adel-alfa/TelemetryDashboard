using TelemetryDashboard.Domain.Entities;

namespace TelemetryDashboard.Application;

/// <summary>Provides an asynchronous stream of telemetry metrics.</summary>
/// <remarks>
/// Consumers in the same process can call this abstraction directly. The web application also exposes
/// the stream through an HTTP Server-Sent Events endpoint for remote clients.
/// </remarks>
public interface ITelemetryStream
{

    /// <summary>Reads telemetry metrics until the stream ends or cancellation is requested.</summary>
    /// <param name="cancellationToken">A token used to stop the asynchronous enumeration.</param>
    /// <returns>An asynchronous sequence of telemetry metrics.</returns>
    /// <remarks>
    /// The in-process contract does not perform HTTP or SSE framing. The web endpoint adapts this sequence
    /// to SSE for out-of-process HTTP clients.
    /// </remarks>
    IAsyncEnumerable<TelemetryMetric> ReadAsync(CancellationToken cancellationToken = default);
}