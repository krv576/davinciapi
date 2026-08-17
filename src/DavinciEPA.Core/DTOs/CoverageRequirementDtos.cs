namespace DavinciEPA.Core.DTOs;

/// <summary>Context supplied to a CRD hook invocation identifying the order and patient in scope.</summary>
public sealed record CoverageRequirementDiscoveryRequestDto(
    string Hook,
    string PatientIdentifier,
    string PayerId,
    string OrderReference,
    string OrderResourceJson,
    IReadOnlyDictionary<string, string> PrefetchResourcesJson);

/// <summary>Result of evaluating coverage requirement rules for a single requirement.</summary>
public sealed record CoverageRequirementResultDto(
    string RequirementCode,
    string RequirementDescription,
    bool IsMet,
    string? DocumentationQuestionnaireCanonicalUrl);

/// <summary>Aggregate discovery result for all requirements evaluated for an order.</summary>
public sealed record CoverageRequirementDiscoveryResultDto(
    string OrderReference,
    IReadOnlyCollection<CoverageRequirementResultDto> Requirements);
