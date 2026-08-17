namespace DavinciEPA.Core.Interfaces.External;

/// <summary>Port for querying a patient's FHIR data from the launching EHR (used for CRD prefetch gaps and DTR pre-population).</summary>
public interface IEhrFhirClient
{
    Task<string?> SearchAsync(
        string fhirServerBaseUrl,
        string? accessToken,
        string resourceType,
        IReadOnlyDictionary<string, string> searchParameters,
        CancellationToken cancellationToken);

    Task<string?> ReadAsync(
        string fhirServerBaseUrl,
        string? accessToken,
        string resourceType,
        string id,
        CancellationToken cancellationToken);
}

/// <summary>Token response from a SMART on FHIR authorization server's token endpoint.</summary>
public sealed record SmartTokenResponse(
    string AccessToken,
    string TokenType,
    int ExpiresInSeconds,
    string? Patient,
    string? Scope,
    string? RefreshToken);

/// <summary>Port for exchanging a SMART App Launch authorization code (with PKCE) for an access token.</summary>
public interface ISmartTokenExchangeClient
{
    Task<SmartTokenResponse> ExchangeAuthorizationCodeAsync(
        string tokenEndpoint,
        string clientId,
        string redirectUri,
        string code,
        string codeVerifier,
        CancellationToken cancellationToken);
}
