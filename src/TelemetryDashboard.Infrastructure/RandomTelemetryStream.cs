using System.Runtime.CompilerServices;
using TelemetryDashboard.Application;
using TelemetryDashboard.Domain.Entities;
using TelemetryDashboard.Domain.ValueObjects;

namespace TelemetryDashboard.Infrastructure;

public sealed class RandomTelemetryStream(TimeProvider timeProvider) : ITelemetryStream
{
    private const long TotalMemoryBytes = 16L * 1024 * 1024 * 1024;

    /// <summary>Asynchronously generates telemetry samples until cancellation is requested.</summary>
    /// <param name="cancellationToken">A token that stops the delay and ends enumeration.</param>
    /// <returns>A stream of simulated metrics, normally one metric per second.</returns>
    /// <remarks>
    /// Cancellation during the sampling delay ends the sequence without yielding another item.
    /// Other generation exceptions are ignored and sampling continues on the next iteration.
    /// </remarks>
    public async IAsyncEnumerable<TelemetryMetric> ReadAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        TelemetryMetric metric = null;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

                var usedMemory = Random.Shared.NextInt64(TotalMemoryBytes / 3, TotalMemoryBytes * 9 / 10);
                metric = new TelemetryMetric(
                   Guid.NewGuid(),
                   timeProvider.GetUtcNow(),
                   decimal.Round((decimal)(Random.Shared.NextDouble() * 72 + 8), 1),
                   new MemoryUsage(usedMemory, TotalMemoryBytes),
                   new NetworkIo(
                       Random.Shared.NextInt64(180_000, 8_000_000),
                       Random.Shared.NextInt64(90_000, 5_000_000)),
                   Random.Shared.Next(4, 180));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }
            catch (Exception ex)
            {                 
                continue;
            }
            
            
            if(metric is not null)
            {
                yield return metric;
            }
            
        }
    }
}