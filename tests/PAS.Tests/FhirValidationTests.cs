using DavinciEPA.Core.Constants;
using DavinciEPA.Fhir.Builders;
using DavinciEPA.Fhir.Serialization;
using DavinciEPA.Fhir.Validation;
using FluentAssertions;

namespace PAS.Tests;

/// <summary>FHIR validation tests: confirms builders produce resources conforming to the pragmatic Da Vinci profile checks in <c>DavinciEPA.Fhir</c>.</summary>
public class FhirValidationTests
{
    private static readonly FhirJsonSerializerService SerializerService = new();

    [Fact]
    public void ClaimBuilder_BuildsAValidPreauthorizationClaim()
    {
        var builder = new ClaimBuilder(SerializerService);
        var claimJson = builder.BuildPreauthorizationClaimJson("patient-1", "payer-1", "70551", Array.Empty<string>());

        var validator = new FhirResourceValidator(SerializerService, new OperationOutcomeBuilder(SerializerService));
        var outcome = validator.Validate(claimJson, "Claim", DaVinciProfiles.PasClaim);

        outcome.IsValid.Should().BeTrue(string.Join("; ", outcome.Issues));
    }

    [Fact]
    public void ClaimResponseBuilder_BuildsAValidClaimResponse()
    {
        var builder = new ClaimResponseBuilder(SerializerService);
        var claimResponseJson = builder.BuildClaimResponseJson("Claim/123", "patient-1", "payer-1", "complete", "Granted", null);

        var validator = new FhirResourceValidator(SerializerService, new OperationOutcomeBuilder(SerializerService));
        var outcome = validator.Validate(claimResponseJson, "ClaimResponse", DaVinciProfiles.PasClaimResponse);

        outcome.IsValid.Should().BeTrue(string.Join("; ", outcome.Issues));
    }

    [Fact]
    public void Validate_WhenResourceTypeMismatched_ReturnsInvalidWithIssue()
    {
        var builder = new ClaimBuilder(SerializerService);
        var claimJson = builder.BuildPreauthorizationClaimJson("patient-1", "payer-1", "70551", Array.Empty<string>());

        var validator = new FhirResourceValidator(SerializerService, new OperationOutcomeBuilder(SerializerService));
        var outcome = validator.Validate(claimJson, "ClaimResponse", DaVinciProfiles.PasClaimResponse);

        outcome.IsValid.Should().BeFalse();
        outcome.Issues.Should().Contain(issue => issue.Contains("resourceType"));
    }
}
