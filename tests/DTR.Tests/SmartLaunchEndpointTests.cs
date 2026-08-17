using System.Net;
using DTR.Tests.Support;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DTR.Tests;

public class SmartLaunchEndpointTests : IClassFixture<DtrApiFactory>
{
    private readonly DtrApiFactory _factory;

    public SmartLaunchEndpointTests(DtrApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Launch_RedirectsToAuthorizeEndpointWithPkceParameters()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/smart/launch?iss=https://ehr.example.org/fhir&launch=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        var location = response.Headers.Location!.ToString();
        location.Should().StartWith("https://ehr.example.org/fhir/authorize");
        location.Should().Contain("code_challenge_method=S256");
    }

    [Fact]
    public async Task GetQuestionnairePackage_WithoutBearerToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            $"/documentation-requirements/{Guid.NewGuid()}/questionnaire-package?patient=123&fhirServer=https://ehr.example.org/fhir");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
