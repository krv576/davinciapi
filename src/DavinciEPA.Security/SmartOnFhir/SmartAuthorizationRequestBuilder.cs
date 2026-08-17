using DavinciEPA.Core.Constants;

namespace DavinciEPA.Security.SmartOnFhir;

/// <summary>Builds the SMART App Launch authorization redirect URL (authorization code + PKCE, per the SMART App Launch framework).</summary>
public sealed class SmartAuthorizationRequestBuilder
{
    public Uri BuildAuthorizationUrl(
        string authorizeEndpoint,
        string clientId,
        string redirectUri,
        string scope,
        string state,
        string launch,
        string audience,
        string codeChallenge)
    {
        var query = new Dictionary<string, string>
        {
            ["response_type"] = "code",
            ["client_id"] = clientId,
            ["redirect_uri"] = redirectUri,
            ["scope"] = scope,
            ["state"] = state,
            [SmartOnFhirConstants.LaunchParam] = launch,
            ["aud"] = audience,
            ["code_challenge"] = codeChallenge,
            ["code_challenge_method"] = SmartOnFhirConstants.CodeChallengeMethodS256
        };

        var queryString = string.Join('&', query.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        var separator = authorizeEndpoint.Contains('?') ? '&' : '?';

        return new Uri($"{authorizeEndpoint}{separator}{queryString}");
    }
}
