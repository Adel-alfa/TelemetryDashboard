namespace TelemetryDashboard.Domain.ValueObjects;

/// <summary>Represents inbound and outbound network throughput in bytes per second.</summary>
public sealed record NetworkIo
{
    /// <summary>Gets inbound network throughput in bytes per second.</summary>
    public long InboundBytesPerSecond { get; }
    /// <summary>Gets outbound network throughput in bytes per second.</summary>
    public long OutboundBytesPerSecond { get; }

    /// <summary>Creates a network throughput value after validating both rates.</summary>
    /// <param name="inboundBytesPerSecond">Inbound bytes per second; must be non-negative.</param>
    /// <param name="outboundBytesPerSecond">Outbound bytes per second; must be non-negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">Either rate is negative.</exception>
    public NetworkIo(long inboundBytesPerSecond, long outboundBytesPerSecond)
    {
        if (inboundBytesPerSecond < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(inboundBytesPerSecond));
        }

        if (outboundBytesPerSecond < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(outboundBytesPerSecond));
        }

        InboundBytesPerSecond = inboundBytesPerSecond;
        OutboundBytesPerSecond = outboundBytesPerSecond;
    }
}