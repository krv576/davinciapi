using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Validation;

/// <summary>
/// Validates raw FHIR JSON against the pragmatic structural/profile expectations of the Da Vinci CRD, DTR,
/// and PAS Implementation Guides (declared <c>Meta.Profile</c> plus required elements for the resource types
/// this platform exchanges). Full StructureDefinition-based validation against the published IG packages can
/// be layered in via an external validator, see docs/testing-strategy.md.
/// </summary>
public sealed class FhirResourceValidator : IFhirResourceValidator
{
    private readonly FhirJsonSerializerService _serializerService;
    private readonly IOperationOutcomeBuilder _operationOutcomeBuilder;

    public FhirResourceValidator(FhirJsonSerializerService serializerService, IOperationOutcomeBuilder operationOutcomeBuilder)
    {
        _serializerService = serializerService;
        _operationOutcomeBuilder = operationOutcomeBuilder;
    }

    public FhirValidationOutcome Validate(string resourceJson, string resourceType, string expectedProfileUrl)
    {
        var issues = new List<string>();
        Resource resource;

        try
        {
            resource = _serializerService.ParseResource(resourceJson);
        }
        catch (Exception ex)
        {
            issues.Add($"Resource could not be parsed as valid FHIR JSON: {ex.Message}");
            return new FhirValidationOutcome(false, issues, _operationOutcomeBuilder.BuildFromValidationFailuresJson(issues));
        }

        if (!string.Equals(resource.TypeName, resourceType, StringComparison.Ordinal))
        {
            issues.Add($"Expected resourceType '{resourceType}' but found '{resource.TypeName}'.");
        }

        var profiles = resource.Meta?.Profile?.ToList() ?? new List<string>();
        if (!profiles.Contains(expectedProfileUrl))
        {
            issues.Add($"Resource does not declare the expected profile '{expectedProfileUrl}' in Meta.Profile.");
        }

        ValidateResourceSpecificRequirements(resource, issues);

        var isValid = issues.Count == 0;
        return new FhirValidationOutcome(
            isValid,
            issues,
            isValid ? null : _operationOutcomeBuilder.BuildFromValidationFailuresJson(issues));
    }

    private static void ValidateResourceSpecificRequirements(Resource resource, List<string> issues)
    {
        switch (resource)
        {
            case Bundle bundle:
                if (bundle.Type is null)
                {
                    issues.Add("Bundle.type is required.");
                }

                if (bundle.Entry is null || bundle.Entry.Count == 0)
                {
                    issues.Add("Bundle.entry must contain at least one entry.");
                }

                break;

            case Claim claim:
                if (claim.Patient is null)
                {
                    issues.Add("Claim.patient is required.");
                }

                if (claim.Insurer is null)
                {
                    issues.Add("Claim.insurer is required.");
                }

                if (claim.Use != ClaimUseCode.Preauthorization)
                {
                    issues.Add("Claim.use must be 'preauthorization' for a PAS submission.");
                }

                break;

            case ClaimResponse claimResponse:
                if (claimResponse.Patient is null)
                {
                    issues.Add("ClaimResponse.patient is required.");
                }

                if (claimResponse.Outcome is null)
                {
                    issues.Add("ClaimResponse.outcome is required.");
                }

                break;

            case QuestionnaireResponse questionnaireResponse:
                if (questionnaireResponse.Status is null)
                {
                    issues.Add("QuestionnaireResponse.status is required.");
                }

                if (string.IsNullOrWhiteSpace(questionnaireResponse.Questionnaire))
                {
                    issues.Add("QuestionnaireResponse.questionnaire is required.");
                }

                break;

            case Questionnaire questionnaire:
                if (string.IsNullOrWhiteSpace(questionnaire.Url))
                {
                    issues.Add("Questionnaire.url is required.");
                }

                if (questionnaire.Status is null)
                {
                    issues.Add("Questionnaire.status is required.");
                }

                break;
        }
    }
}
