using System.Text.Json;
using System.Text.Json.Serialization;
using DavinciEPA.Core.Exceptions;
using DavinciEPA.Core.Interfaces.External;

namespace DavinciEPA.Infrastructure.ExternalClients;

/// <summary>Exchanges a SMART App Launch authorization code (with PKCE) for an access token, per the SMART App Launch framework.</summary>
public sealed class SmartTokenExchangeClient : ISmartTokenExchangeClient
{
    private readonly HttpClient _httpClient;

    public SmartTokenExchangeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SmartTokenResponse> ExchangeAuthorizationCodeAsync(
        string tokenEndpoint,
        string clientId,
        string redirectUri,
        string code,
        string codeVerifier,
        CancellationToken cancellationToken)
    {
        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = redirectUri,
            ["client_id"] = clientId,
            ["code_verifier"] = codeVerifier
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(form)
        };

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ExternalServiceException(
                    "SmartTokenEndpoint",
                    $"Token exchange failed with status {(int)response.StatusCode}.");
            }

            var payload = JsonSerializer.Deserialize<TokenResponsePayload>(body)
                ?? throw new ExternalServiceException("SmartTokenEndpoint", "Token endpoint returned an empty response.");

            return new SmartTokenResponse(
                payload.AccessToken ?? throw new ExternalServiceException("SmartTokenEndpoint", "Token response did not include an access_token."),
                payload.TokenType ?? "Bearer",
                payload.ExpiresIn,
                payload.Patient,
                payload.Scope,
                payload.RefreshToken);
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException("SmartTokenEndpoint", "Failed to reach the SMART token endpoint.", ex);
        }
    }

    private sealed class TokenResponsePayload
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("patient")]
        public string? Patient { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
