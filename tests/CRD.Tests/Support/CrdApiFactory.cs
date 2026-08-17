using DavinciEPA.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CRD.Tests.Support;

/// <summary>Boots DavinciEPA.CRD.Api in-process for integration tests, swapping the SQL Server EF Core provider for an isolated in-memory database.</summary>
public sealed class CrdApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DavinciEpaDbContext>>();
            services.AddDbContext<DavinciEpaDbContext>(options =>
                options.UseInMemoryDatabase($"crd-tests-{Guid.NewGuid()}"));
        });
    }
}
