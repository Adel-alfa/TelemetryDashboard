using TelemetryDashboard.Domain.ValueObjects;

namespace TelemetryDashboard.Domain.Entities;

public sealed record TelemetryMetric
{
    public Guid Id { get; }
    public DateTimeOffset CapturedAt { get; }
    public decimal CpuPercent { get; }
    public MemoryUsage Memory { get; }
    public NetworkIo Network { get; }
    public int ActiveRequests { get; }

    public TelemetryMetric(
        Guid id,
        DateTimeOffset capturedAt,
        decimal cpuPercent,
        MemoryUsage memory,
        NetworkIo network,
        int activeRequests)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("A telemetry metric must have an identifier.", nameof(id));
        }

        if (cpuPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(cpuPercent), "CPU usage must be between zero and one hundred.");
        }

        if (activeRequests < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(activeRequests));
        }

        Id = id;
        CapturedAt = capturedAt;
        CpuPercent = cpuPercent;
        Memory = memory ?? throw new ArgumentNullException(nameof(memory));
        Network = network ?? throw new ArgumentNullException(nameof(network));
        ActiveRequests = activeRequests;
    }
}