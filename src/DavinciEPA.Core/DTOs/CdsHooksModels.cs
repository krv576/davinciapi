using System.Text.Json.Serialization;

namespace DavinciEPA.Core.DTOs;

/// <summary>CDS Hooks discovery document entry describing one supported hook/service.</summary>
public sealed record CdsServiceDefinitionDto(
    string Id,
    string Hook,
    string Title,
    string Description,
    IReadOnlyDictionary<string, string>? Prefetch,
    CdsHooksServiceExtensionDto? Extension = null);

/// <summary>CRD-defined <c>extension</c> object on a discovery service entry (IG §10.4).</summary>
public sealed record CdsHooksServiceExtensionDto(
    [property: JsonPropertyName("davinci-crd.configuration-options")]
    IReadOnlyCollection<CdsHooksConfigurationOptionDto> ConfigurationOptions,
    [property: JsonPropertyName("davinci-crd.version")]
    IReadOnlyCollection<string>? Version = null);

/// <summary>A single client-configurable option advertised for a CRD service (IG §10.4).</summary>
public sealed record CdsHooksConfigurationOptionDto(
    string Code,
    string Type,
    string Name,
    string Description,
    bool Default);

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

public sealed record CdsHooksSourceDto(string Label, string? Url, CdsHooksCodingDto Topic);

/// <summary>Coding used for <c>Card.source.topic</c>, extensibly bound to the CRD Card Types ValueSet.</summary>
public sealed record CdsHooksCodingDto(string System, string Code, string? Display);

public sealed record CdsHooksLinkDto(string Label, string Url, string Type);

/// <summary>Full CDS Hooks response returned from a hook invocation.</summary>
public sealed record CdsHooksResponseDto(IReadOnlyCollection<CdsHooksCardDto> Cards);
