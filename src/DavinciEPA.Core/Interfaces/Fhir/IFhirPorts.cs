namespace DavinciEPA.Core.Interfaces.Fhir;

/// <summary>Result of validating a FHIR resource against its declared/expected profile.</summary>
public sealed record FhirValidationOutcome(bool IsValid, IReadOnlyCollection<string> Issues, string? OperationOutcomeJson);

/// <summary>
/// Port for validating a raw FHIR resource (as JSON) against Da Vinci profile constraints.
/// Deliberately string-based so that <c>DavinciEPA.Core</c> never needs a dependency on the Firely SDK;
/// the implementation in <c>DavinciEPA.Fhir</c> performs the actual typed parsing/validation.
/// </summary>
public interface IFhirResourceValidator
{
    FhirValidationOutcome Validate(string resourceJson, string resourceType, string expectedProfileUrl);
}

/// <summary>Port for building a FHIR <c>OperationOutcome</c> for validation/processing failures.</summary>
public interface IOperationOutcomeBuilder
{
    string BuildErrorJson(string diagnostics, IReadOnlyCollection<string>? issues = null);

    string BuildFromValidationFailuresJson(IReadOnlyCollection<string> failures);
}

/// <summary>Port for constructing a PAS prior-authorization <c>Claim</c> resource from a domain request.</summary>
public interface IClaimBuilder
{
    string BuildPreauthorizationClaimJson(
        string patientIdentifier,
        string payerId,
        string orderReference,
        IReadOnlyCollection<string> supportingInfoReferences);
}

/// <summary>Port for constructing a PAS <c>ClaimResponse</c> resource representing an adjudication decision.</summary>
public interface IClaimResponseBuilder
{
    string BuildClaimResponseJson(
        string claimReference,
        string patientIdentifier,
        string payerId,
        string outcome,
        string? disposition,
        string? taskIdentifier);
}

/// <summary>
/// Port for constructing a DTR <c>Questionnaire</c> resource for a given coverage requirement. The implementation
/// owns the catalog of known questionnaire item definitions keyed by canonical URL.
/// </summary>
public interface IQuestionnaireBuilder
{
    string BuildQuestionnaireJson(string canonicalUrl, string title);
}

/// <summary>Port for assembling FHIR <c>Bundle</c> resources (collection, transaction, searchset) for the various IG interactions.</summary>
public interface IBundleBuilder
{
    string BuildCollectionBundleJson(IReadOnlyCollection<string> resourceJsonEntries);

    string BuildSearchsetBundleJson(IReadOnlyCollection<string> resourceJsonEntries, int total);
}
