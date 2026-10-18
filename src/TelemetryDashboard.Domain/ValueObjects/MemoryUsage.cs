namespace TelemetryDashboard.Domain.ValueObjects;

/// <summary>Represents used and total physical memory in bytes.</summary>
public sealed record MemoryUsage
{
    /// <summary>Gets the number of bytes currently used.</summary>
    public long UsedBytes { get; }
    /// <summary>Gets the total amount of physical memory in bytes.</summary>
    public long TotalBytes { get; }
    /// <summary>Gets the used memory as a percentage of the total.</summary>
    public double UsagePercent => (double)UsedBytes / TotalBytes * 100;

    /// <summary>Creates a memory usage value after validating its byte counts.</summary>
    /// <param name="usedBytes">Used memory in the inclusive range zero through <paramref name="totalBytes"/>.</param>
    /// <param name="totalBytes">Total available memory; must be greater than zero.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="totalBytes"/> is not positive or <paramref name="usedBytes"/> is outside its valid range.</exception>
    public MemoryUsage(long usedBytes, long totalBytes)
    {
        if (totalBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalBytes), "Total memory must be positive.");
        }

        if (usedBytes < 0 || usedBytes > totalBytes)
        {
            throw new ArgumentOutOfRangeException(nameof(usedBytes), "Used memory must be between zero and total memory.");
        }

        UsedBytes = usedBytes;
        TotalBytes = totalBytes;
    }
}