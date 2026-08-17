using System.Security.Claims;
using DavinciEPA.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DavinciEPA.Security.Authorization;

/// <summary>Requires that the authenticated principal's token carries a specific OAuth2/SMART scope.</summary>
public sealed class ScopeRequirement : IAuthorizationRequirement
{
    public ScopeRequirement(string scope)
    {
        Scope = scope;
    }

    public string Scope { get; }
}

/// <summary>Evaluates <see cref="ScopeRequirement"/> against the <c>scope</c> claim on the current principal.</summary>
public sealed class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ScopeRequirement requirement)
    {
        if (HasRequirementScope(context.User, requirement.Scope))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool HasRequirementScope(ClaimsPrincipal user, string scope) => user.HasScope(scope);
}
