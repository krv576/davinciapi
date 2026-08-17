using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace DavinciEPA.Security.Authorization;

/// <summary>Well-known authorization policy names used across the CRD, DTR, and PAS APIs.</summary>
public static class PolicyNames
{
    /// <summary>CDS Hooks service-to-service invocation (CRD).</summary>
    public const string CdsHooksInvoke = "CdsHooksInvoke";

    /// <summary>SMART App Launch context established for the DTR questionnaire app.</summary>
    public const string SmartLaunch = "SmartLaunch";

    /// <summary>Patient-scoped read access (DTR pre-population reads from the EHR).</summary>
    public const string PatientRead = "PatientRead";

    /// <summary>SMART Backend Services scope required to submit a prior authorization (PAS $submit).</summary>
    public const string SystemClaimWrite = "SystemClaimWrite";

    /// <summary>SMART Backend Services scope required to read prior authorization status (PAS $inquire).</summary>
    public const string SystemClaimRead = "SystemClaimRead";
}

/// <summary>Registers the scope-based authorization policies and their backing requirement handler.</summary>
public static class AuthorizationPolicyExtensions
{
    private const string LaunchScope = "launch";
    private const string PatientReadAllScope = "patient/*.read";
    private const string SystemClaimWriteScope = "system/Claim.write";
    private const string SystemClaimReadScope = "system/Claim.read";

    public static IServiceCollection AddDavinciAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyNames.CdsHooksInvoke, policy => policy.RequireAuthenticatedUser())
            .AddPolicy(PolicyNames.SmartLaunch, policy => policy.Requirements.Add(new ScopeRequirement(LaunchScope)))
            .AddPolicy(PolicyNames.PatientRead, policy => policy.Requirements.Add(new ScopeRequirement(PatientReadAllScope)))
            .AddPolicy(PolicyNames.SystemClaimWrite, policy => policy.Requirements.Add(new ScopeRequirement(SystemClaimWriteScope)))
            .AddPolicy(PolicyNames.SystemClaimRead, policy => policy.Requirements.Add(new ScopeRequirement(SystemClaimReadScope)));

        return services;
    }
}
