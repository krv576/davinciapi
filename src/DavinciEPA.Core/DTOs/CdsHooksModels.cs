namespace DavinciEPA.Core.DTOs;

/// <summary>CDS Hooks discovery document entry describing one supported hook/service.</summary>
public sealed record CdsServiceDefinitionDto(
    string Id,
    string Hook,
    string Title,
    string Description,
    CdsHooksPrefetchDto? Prefetch);

/// <summary>Prefetch template keyed by token, per the CDS Hooks specification.</summary>
public sealed record CdsHooksPrefetchDto(IReadOnlyDictionary<string, string> Queries);

/// <summary>Inbound CDS Hooks request payload for a single hook invocation.</summary>
public sealed record CdsHooksRequestDto(
    string Hook,
    string HookInstance,
    string FhirServer,
    string? FhirAuthorizationToken,
    CdsHooksContextDto Context,
    IReadOnlyDictionary<string, string>? Prefetch);

/// <summary>Hook-specific context, e.g. patientId/userId/selections for order-select and order-sign.</summary>
public sealed record CdsHooksContextDto(
    string PatientId,
    string UserId,
    string? EncounterId,
    IReadOnlyCollection<string> Selections,
    IReadOnlyDictionary<string, string> Draft);

/// <summary>A single CDS Hooks response card.</summary>
public sealed record CdsHooksCardDto(
    string Summary,
    string Indicator,
    string Detail,
    CdsHooksSourceDto Source,
    IReadOnlyCollection<CdsHooksLinkDto> Links);

public sealed record CdsHooksSourceDto(string Label, string? Url);

public sealed record CdsHooksLinkDto(string Label, string Url, string Type);

/// <summary>Full CDS Hooks response returned from a hook invocation.</summary>
public sealed record CdsHooksResponseDto(IReadOnlyCollection<CdsHooksCardDto> Cards);
