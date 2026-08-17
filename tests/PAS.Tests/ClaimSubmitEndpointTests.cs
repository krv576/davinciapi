using System.Net;
using System.Text;
using PAS.Tests.Support;
using FluentAssertions;

namespace PAS.Tests;

public class ClaimSubmitEndpointTests : IClassFixture<PasApiFactory>
{
    private readonly PasApiFactory _factory;

    public ClaimSubmitEndpointTests(PasApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Submit_WithoutBearerToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var content = new StringContent("{}", Encoding.UTF8, "application/fhir+json");

        var response = await client.PostAsync("/Claim/$submit", content);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetStatus_WithoutBearerToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Claim/unknown-id/status");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
