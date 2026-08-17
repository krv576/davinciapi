namespace DavinciEPA.Infrastructure.Configuration;

/// <summary>Database connectivity settings, bound from configuration (never hard-coded).</summary>
public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string ConnectionString { get; set; } = string.Empty;

    public int CommandTimeoutSeconds { get; set; } = 30;
}
