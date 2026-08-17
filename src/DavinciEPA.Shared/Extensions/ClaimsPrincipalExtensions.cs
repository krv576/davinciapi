using System.Security.Claims;

namespace DavinciEPA.Shared.Extensions;

/// <summary>Convenience accessors for claims commonly present on OAuth2/SMART on FHIR access tokens.</summary>
public static class ClaimsPrincipalExtensions
{
    private const string SubjectClaimType = "sub";
    private const string ClientIdClaimType = "client_id";
    private const string AzpClaimType = "azp";
    private const string ScopeClaimType = "scope";
    private const string FhirUserClaimType = "fhirUser";

    public static string? GetSubjectId(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(SubjectClaimType) ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static string? GetClientId(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClientIdClaimType) ?? principal.FindFirstValue(AzpClaimType);

    public static string? GetFhirUser(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(FhirUserClaimType);

    public static bool HasScope(this ClaimsPrincipal principal, string scope)
    {
        var scopesClaim = principal.FindFirstValue(ScopeClaimType);
        if (string.IsNullOrWhiteSpace(scopesClaim))
        {
            return false;
        }

        return scopesClaim
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Contains(scope, StringComparer.OrdinalIgnoreCase);
    }
}
