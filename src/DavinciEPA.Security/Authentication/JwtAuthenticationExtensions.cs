using DavinciEPA.Security.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DavinciEPA.Security.Authentication;

/// <summary>Configures OAuth2/OIDC JWT bearer authentication shared by all three Da Vinci APIs.</summary>
public static class JwtAuthenticationExtensions
{
    /// <summary>
    /// Registers JWT bearer authentication. Signature, issuer, audience, and lifetime validation are always
    /// enabled; callers must never disable them, including in Development.
    /// </summary>
    public static IServiceCollection AddDavinciJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = JwtBearerSettings.SectionName)
    {
        var settings = configuration.GetSection(sectionName).Get<JwtBearerSettings>() ?? new JwtBearerSettings();

        if (string.IsNullOrWhiteSpace(settings.Authority))
        {
            throw new InvalidOperationException(
                $"Configuration section '{sectionName}:Authority' must be set before authentication can be configured.");
        }

        services.Configure<JwtBearerSettings>(configuration.GetSection(sectionName));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = settings.Authority;
                options.Audience = settings.Audience;
                options.RequireHttpsMetadata = settings.RequireHttpsMetadata;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers = settings.ValidIssuers.Length > 0
                        ? settings.ValidIssuers
                        : new[] { settings.Authority },
                    ValidateAudience = !string.IsNullOrWhiteSpace(settings.Audience),
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.FromSeconds(settings.ClockSkewSeconds)
                };

                options.Events = new JwtBearerEvents
                {
                    // Never log the token itself, only the failure reason and correlation-relevant metadata.
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("DavinciEPA.Security.JwtBearer");
                        logger.LogWarning(
                            "JWT bearer authentication failed for {Path}: {Reason}",
                            context.HttpContext.Request.Path,
                            context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}
