using DavinciEPA.Security.Authentication;
using DavinciEPA.Security.Authorization;
using DavinciEPA.Security.ClientCredentials;
using DavinciEPA.Security.Configuration;
using DavinciEPA.Security.SmartOnFhir;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DavinciEPA.Security.Extensions;

/// <summary>Single composition-root entry point wiring up authentication, authorization, SMART on FHIR, and client-credentials support.</summary>
public static class SecurityServiceCollectionExtensions
{
    public static IServiceCollection AddDavinciSecurity(
        this IServiceCollection services,
        IConfiguration configuration,
        string jwtSectionName = JwtBearerSettings.SectionName)
    {
        services.AddDavinciJwtAuthentication(configuration, jwtSectionName);
        services.AddAuthorization();
        services.AddDavinciAuthorizationPolicies();

        services.Configure<ClientCredentialsSettings>(configuration.GetSection(ClientCredentialsSettings.SectionName));
        services.AddSingleton<IClientAssertionGenerator, JwtClientAssertionGenerator>();

        services.AddSingleton<SmartAuthorizationRequestBuilder>();

        return services;
    }
}
