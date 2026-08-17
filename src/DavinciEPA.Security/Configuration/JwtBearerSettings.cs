namespace DavinciEPA.Security.Configuration;

/// <summary>Strongly-typed OAuth2/OIDC bearer token validation settings, bound from configuration.</summary>
public sealed class JwtBearerSettings
{
    public const string SectionName = "Authentication:Jwt";

    /// <summary>OIDC authority (issuer) metadata endpoint base, e.g. the EHR's or payer's identity provider.</summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>Expected audience (this API's resource identifier) required on inbound tokens.</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Additional issuers accepted beyond <see cref="Authority"/> (e.g. multiple trusted EHRs for CDS Hooks).</summary>
    public string[] ValidIssuers { get; set; } = Array.Empty<string>();

    /// <summary>Must remain <c>true</c> outside local development; enforced by <see cref="JwtAuthenticationExtensions"/>.</summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    public int ClockSkewSeconds { get; set; } = 60;
}
