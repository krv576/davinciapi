using System.Net.Http.Headers;
using DavinciEPA.Core.Exceptions;
using DavinciEPA.Core.Interfaces.External;
using Microsoft.Extensions.Logging;

namespace DavinciEPA.Infrastructure.ExternalClients;

/// <summary>Outbound HTTP client for reading/searching FHIR resources on the launching EHR's FHIR server.</summary>
public sealed class EhrFhirClient : IEhrFhirClient
{
    private const string FhirJsonMediaType = "application/fhir+json";

    private readonly HttpClient _httpClient;
    private readonly ILogger<EhrFhirClient> _logger;

    public EhrFhirClient(HttpClient httpClient, ILogger<EhrFhirClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public Task<string?> SearchAsync(
        string fhirServerBaseUrl,
        string? accessToken,
        string resourceType,
        IReadOnlyDictionary<string, string> searchParameters,
        CancellationToken cancellationToken)
    {
        var query = string.Join(
            '&',
            searchParameters.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

        var uri = new Uri(new Uri(EnsureTrailingSlash(fhirServerBaseUrl)), $"{resourceType}?{query}");
        return SendAsync(uri, accessToken, resourceType, cancellationToken);
    }

    public Task<string?> ReadAsync(
        string fhirServerBaseUrl,
        string? accessToken,
        string resourceType,
        string id,
        CancellationToken cancellationToken)
    {
        var uri = new Uri(new Uri(EnsureTrailingSlash(fhirServerBaseUrl)), $"{resourceType}/{id}");
        return SendAsync(uri, accessToken, resourceType, cancellationToken);
    }

    private async Task<string?> SendAsync(Uri uri, string? accessToken, string resourceType, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(FhirJsonMediaType));

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "EHR FHIR server returned {StatusCode} for a {ResourceType} query.",
                    (int)response.StatusCode,
                    resourceType);
                return null;
            }

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException(
                "EhrFhirServer",
                $"Failed to query {resourceType} from the EHR FHIR server.",
                ex);
        }
    }

    private static string EnsureTrailingSlash(string url) => url.EndsWith('/') ? url : url + "/";
}
