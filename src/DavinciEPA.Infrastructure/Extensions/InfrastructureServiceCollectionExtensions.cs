using DavinciEPA.Core.Interfaces.External;
using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Infrastructure.Configuration;
using DavinciEPA.Infrastructure.ExternalClients;
using DavinciEPA.Infrastructure.Persistence;
using DavinciEPA.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DavinciEPA.Infrastructure.Extensions;

/// <summary>Single composition-root entry point wiring up EF Core persistence, repositories, and outbound HTTP clients.</summary>
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddDavinciInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
            ?? new DatabaseOptions();

        if (string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))
        {
            throw new InvalidOperationException(
                $"Configuration section '{DatabaseOptions.SectionName}:ConnectionString' must be set before infrastructure can be configured.");
        }

        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<DavinciEpaDbContext>(options =>
            options.UseSqlServer(
                databaseOptions.ConnectionString,
                sqlOptions => sqlOptions.CommandTimeout(databaseOptions.CommandTimeoutSeconds)));

        services.AddScoped<IPriorAuthorizationRequestRepository, PriorAuthorizationRequestRepository>();
        services.AddScoped<ICoverageRequirementRepository, CoverageRequirementRepository>();
        services.AddScoped<IDocumentationRequirementRepository, DocumentationRequirementRepository>();
        services.AddScoped<IRuleEvaluationLogRepository, RuleEvaluationLogRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHttpClient<IEhrFhirClient, EhrFhirClient>();
        services.AddHttpClient<ISmartTokenExchangeClient, SmartTokenExchangeClient>();

        return services;
    }
}
