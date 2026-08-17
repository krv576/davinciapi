namespace DavinciEPA.Security.Configuration;

/// <summary>
/// SMART Backend Services / PAS client-credentials settings: this system's own identity when acting as an
/// OAuth2 client calling out to a payer or another backend service.
/// </summary>
public sealed class ClientCredentialsSettings
{
    public const string SectionName = "Authentication:ClientCredentials";

    public string ClientId { get; set; } = string.Empty;

    public string TokenEndpoint { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>PEM-encoded RSA private key used to sign the client assertion JWT. Sourced from a vault/secret store, never source control.</summary>
    public string SigningKeyPem { get; set; } = string.Empty;

    public string SigningKeyId { get; set; } = string.Empty;

    public int AssertionLifetimeSeconds { get; set; } = 300;
}
