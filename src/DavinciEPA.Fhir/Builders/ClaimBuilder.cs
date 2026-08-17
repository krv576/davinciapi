using DavinciEPA.Core.Constants;
using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Builders;

/// <summary>Builds a Da Vinci PAS prior-authorization <c>Claim</c> resource (<c>use = preauthorization</c>).</summary>
public sealed class ClaimBuilder : IClaimBuilder
{
    private readonly FhirJsonSerializerService _serializerService;

    public ClaimBuilder(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public string BuildPreauthorizationClaimJson(
        string patientIdentifier,
        string payerId,
        string orderReference,
        IReadOnlyCollection<string> supportingInfoReferences)
    {
        var claim = new Claim
        {
            Id = Guid.NewGuid().ToString("N"),
            Meta = new Meta { Profile = new[] { DaVinciProfiles.PasClaim } },
            Status = FinancialResourceStatusCodes.Active,
            Type = new CodeableConcept(FhirCodeSystems.ClaimType, "institutional"),
            Use = ClaimUseCode.Preauthorization,
            Patient = new ResourceReference($"Patient/{patientIdentifier}"),
            Created = DateTimeOffset.UtcNow.ToString("O"),
            Insurer = new ResourceReference($"Organization/{payerId}"),
            Provider = new ResourceReference("Organization/requesting-provider"),
            Priority = new CodeableConcept(FhirCodeSystems.ProcessPriority, "normal")
        };

        claim.Item.Add(new Claim.ItemComponent
        {
            Sequence = 1,
            ProductOrService = new CodeableConcept(FhirCodeSystems.Cpt, orderReference)
        });

        var sequence = 1;
        foreach (var reference in supportingInfoReferences)
        {
            claim.SupportingInfo.Add(new Claim.SupportingInformationComponent
            {
                Sequence = sequence++,
                Category = new CodeableConcept(
                    "http://terminology.hl7.org/CodeSystem/claiminformationcategory",
                    "info"),
                Value = new ResourceReference(reference)
            });
        }

        return _serializerService.Serialize(claim);
    }
}
