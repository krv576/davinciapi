using DavinciEPA.Core.Constants;
using DavinciEPA.Core.Enums;
using DavinciEPA.Fhir.Builders;
using DavinciEPA.Fhir.Mapping;
using DavinciEPA.Fhir.Serialization;
using DavinciEPA.Fhir.Validation;
using DavinciEPA.Rules.PriorAuthorization;
using FluentAssertions;

namespace Integration.Tests;

/// <summary>
/// Exercises the Fhir + Rules layers together end-to-end for a full PAS submission scenario. Per-API HTTP
/// integration tests (via <c>WebApplicationFactory</c>) live alongside each API in CRD.Tests/DTR.Tests/PAS.Tests,
/// since a single test project cannot unambiguously reference multiple top-level <c>Program</c> entry points
/// without extern aliases; this project instead covers cross-layer domain workflows that span multiple
/// <c>src/</c> projects at once.
/// </summary>
public class PriorAuthorizationWorkflowTests
{
    [Fact]
    public async Task FullSubmissionWorkflow_BuildsValidatesAndAdjudicatesAClaimPendingDocumentation()
    {
        var serializerService = new FhirJsonSerializerService();
        var claimBuilder = new ClaimBuilder(serializerService);
        var bundleBuilder = new BundleBuilder(serializerService);
        var operationOutcomeBuilder = new OperationOutcomeBuilder(serializerService);
        var validator = new FhirResourceValidator(serializerService, operationOutcomeBuilder);
        var ruleEngine = new PriorAuthorizationRuleEngine(
            new PasBundleExtractor(serializerService),
            new PrefetchResourceInspector(serializerService));

        var claimJson = claimBuilder.BuildPreauthorizationClaimJson("patient-1", "payer-1", "70551", Array.Empty<string>());
        var bundleJson = bundleBuilder.BuildCollectionBundleJson(new[] { claimJson });

        var validation = validator.Validate(bundleJson, "Bundle", DaVinciProfiles.PasBundle);
        validation.IsValid.Should().BeTrue(string.Join("; ", validation.Issues));

        var decision = await ruleEngine.EvaluateAsync(bundleJson, Array.Empty<string>(), CancellationToken.None);

        decision.Disposition.Should().Be(PriorAuthorizationDisposition.Pending);
    }

    [Fact]
    public async Task FullSubmissionWorkflow_WithSupportingDiagnosis_IsGrantedAndClaimResponseIsValid()
    {
        var serializerService = new FhirJsonSerializerService();
        var claimBuilder = new ClaimBuilder(serializerService);
        var bundleBuilder = new BundleBuilder(serializerService);
        var claimResponseBuilder = new ClaimResponseBuilder(serializerService);
        var operationOutcomeBuilder = new OperationOutcomeBuilder(serializerService);
        var validator = new FhirResourceValidator(serializerService, operationOutcomeBuilder);
        var ruleEngine = new PriorAuthorizationRuleEngine(
            new PasBundleExtractor(serializerService),
            new PrefetchResourceInspector(serializerService));

        var claimJson = claimBuilder.BuildPreauthorizationClaimJson("patient-1", "payer-1", "70551", Array.Empty<string>());
        var bundleJson = bundleBuilder.BuildCollectionBundleJson(new[] { claimJson });

        var condition = new Hl7.Fhir.Model.Condition
        {
            Code = new Hl7.Fhir.Model.CodeableConcept("http://hl7.org/fhir/sid/icd-10-cm", "M54.5")
        };
        var conditionJson = serializerService.Serialize(condition);

        var decision = await ruleEngine.EvaluateAsync(bundleJson, new[] { conditionJson }, CancellationToken.None);
        decision.Disposition.Should().Be(PriorAuthorizationDisposition.Granted);

        var claimResponseJson = claimResponseBuilder.BuildClaimResponseJson(
            "claim-1", "patient-1", "payer-1", "complete", decision.Disposition.ToString(), null);

        var validation = validator.Validate(claimResponseJson, "ClaimResponse", DaVinciProfiles.PasClaimResponse);
        validation.IsValid.Should().BeTrue(string.Join("; ", validation.Issues));
    }
}
