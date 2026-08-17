using DavinciEPA.Core.Enums;
using DavinciEPA.Fhir.Builders;
using DavinciEPA.Fhir.Mapping;
using DavinciEPA.Fhir.Serialization;
using DavinciEPA.Rules.PriorAuthorization;
using FluentAssertions;
using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace PAS.Tests;

public class PriorAuthorizationRuleEngineTests
{
    private static readonly FhirJsonSerializerService SerializerService = new();

    private static string BuildSubmissionBundle(string procedureCode)
    {
        var claimJson = new ClaimBuilder(SerializerService)
            .BuildPreauthorizationClaimJson("patient-1", "payer-1", procedureCode, Array.Empty<string>());

        return new BundleBuilder(SerializerService).BuildCollectionBundleJson(new[] { claimJson });
    }

    private static string BuildConditionJson(string code)
    {
        var condition = new Condition { Code = new CodeableConcept("http://hl7.org/fhir/sid/icd-10-cm", code) };
        return SerializerService.Serialize(condition);
    }

    private static PriorAuthorizationRuleEngine CreateEngine() =>
        new(new PasBundleExtractor(SerializerService), new PrefetchResourceInspector(SerializerService));

    [Fact]
    public async Task EvaluateAsync_UnrelatedProcedure_AutoApproves()
    {
        var decision = await CreateEngine().EvaluateAsync(BuildSubmissionBundle("99999"), Array.Empty<string>(), CancellationToken.None);

        decision.Disposition.Should().Be(PriorAuthorizationDisposition.Granted);
    }

    [Fact]
    public async Task EvaluateAsync_MatchingProcedureWithoutSupportingInfo_ReturnsPending()
    {
        var decision = await CreateEngine().EvaluateAsync(BuildSubmissionBundle("70551"), Array.Empty<string>(), CancellationToken.None);

        decision.Disposition.Should().Be(PriorAuthorizationDisposition.Pending);
    }

    [Fact]
    public async Task EvaluateAsync_MatchingProcedureWithQualifyingDiagnosis_ReturnsGranted()
    {
        var decision = await CreateEngine().EvaluateAsync(
            BuildSubmissionBundle("70551"),
            new[] { BuildConditionJson("M54.5") },
            CancellationToken.None);

        decision.Disposition.Should().Be(PriorAuthorizationDisposition.Granted);
    }

    [Fact]
    public async Task EvaluateAsync_MatchingProcedureWithoutQualifyingDiagnosis_ReturnsDenied()
    {
        var decision = await CreateEngine().EvaluateAsync(
            BuildSubmissionBundle("70551"),
            new[] { BuildConditionJson("Z00.00") },
            CancellationToken.None);

        decision.Disposition.Should().Be(PriorAuthorizationDisposition.Denied);
    }
}
