namespace DavinciEPA.Security.SmartOnFhir;

/// <summary>The resolved SMART App Launch context for a single DTR app session.</summary>
public sealed record SmartLaunchContext(
    string PatientId,
    string? EncounterId,
    string FhirServerBaseUrl,
    string AccessToken,
    DateTimeOffset ExpiresAt,
    IReadOnlyCollection<string> GrantedScopes);

/// <summary>Parses and validates SMART on FHIR/OAuth2 scope strings.</summary>
public static class SmartScopeParser
{
    public static IReadOnlyCollection<string> Parse(string? scopeString) =>
        string.IsNullOrWhiteSpace(scopeString)
            ? Array.Empty<string>()
            : scopeString.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    public static bool Contains(string? scopeString, string requiredScope) =>
        Parse(scopeString).Contains(requiredScope, StringComparer.OrdinalIgnoreCase);
}
