using DavinciEPA.Core.DTOs;
using DavinciEPA.Fhir.Mapping;
using DavinciEPA.Fhir.Serialization;
using DavinciEPA.Rules.Coverage;
using FluentAssertions;
using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace CRD.Tests;

public class CoverageRuleEngineTests
{
    private static readonly FhirJsonSerializerService SerializerService = new();

    private static string BuildServiceRequestJson(string code)
    {
        var serviceRequest = new ServiceRequest
        {
            Status = RequestStatus.Active,
            Intent = RequestIntent.Order,
            Code = new CodeableConcept("http://www.ama-assn.org/go/cpt", code)
        };

        return SerializerService.Serialize(serviceRequest);
    }

    [Fact]
    public async Task EvaluateAsync_ForAdvancedImagingCode_ReturnsUnmetRequirement()
    {
        var engine = new CoverageRuleEngine(new OrderCodeExtractor(SerializerService));
        var request = new CoverageRequirementDiscoveryRequestDto(
            "order-select",
            "patient-1",
            "payer-1",
            "order-1",
            BuildServiceRequestJson("70551"),
            new Dictionary<string, string>());

        var results = await engine.EvaluateAsync(request, CancellationToken.None);

        results.Should().ContainSingle(r => r.RequirementCode == "PA-IMAGING-ADVANCED" && !r.IsMet);
    }

    [Fact]
    public async Task EvaluateAsync_ForUnrelatedCode_ReturnsNoRequirement()
    {
        var engine = new CoverageRuleEngine(new OrderCodeExtractor(SerializerService));
        var request = new CoverageRequirementDiscoveryRequestDto(
            "order-select",
            "patient-1",
            "payer-1",
            "order-1",
            BuildServiceRequestJson("99999"),
            new Dictionary<string, string>());

        var results = await engine.EvaluateAsync(request, CancellationToken.None);

        results.Should().ContainSingle(r => r.RequirementCode == "PA-NOT-REQUIRED" && r.IsMet);
    }
}
