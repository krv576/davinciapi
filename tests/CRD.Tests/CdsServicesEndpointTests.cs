using System.Net;
using System.Net.Http.Json;
using CRD.Tests.Support;
using FluentAssertions;

namespace CRD.Tests;

public class CdsServicesEndpointTests : IClassFixture<CrdApiFactory>
{
    private readonly CrdApiFactory _factory;

    public CdsServicesEndpointTests(CrdApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCdsServices_ReturnsDiscoveryDocumentWithOrderSelectAndOrderSign()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/cds-services");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("order-select");
        body.Should().Contain("order-sign");
    }

    [Fact]
    public async Task InvokeCdsService_WithoutBearerToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/cds-services/prior-auth-coverage-requirements", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
