namespace DavinciEPA.Shared.Utilities;

/// <summary>Abstraction over the current time, so services can be unit tested without depending on <see cref="DateTimeOffset.UtcNow"/> directly.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

/// <summary>Default <see cref="IClock"/> implementation backed by the system clock.</summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
