using DavinciEPA.Core.Enums;

namespace DavinciEPA.Core.DTOs;

/// <summary>Application-layer request to create a new prior authorization request from an ordering workflow.</summary>
public sealed record CreatePriorAuthorizationRequestDto(
    string PatientIdentifier,
    string PayerId,
    string OrderReference);

/// <summary>Snapshot of a prior authorization request returned to API callers.</summary>
public sealed record PriorAuthorizationRequestDto(
    Guid Id,
    string ExternalId,
    string PatientIdentifier,
    string PayerId,
    string OrderReference,
    PriorAuthorizationStatus Status,
    PriorAuthorizationDisposition? Disposition,
    string? DispositionReason,
    string? TaskIdentifier,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

/// <summary>Request to submit a prior authorization Bundle ($submit) for adjudication.</summary>
public sealed record SubmitPriorAuthorizationDto(
    string PatientIdentifier,
    string PayerId,
    string OrderReference,
    string ClaimReference,
    string RawBundleJson);

/// <summary>Outcome of submitting/adjudicating a prior authorization request.</summary>
public sealed record PriorAuthorizationDecisionDto(
    Guid PriorAuthorizationRequestId,
    PriorAuthorizationDisposition Disposition,
    string? Reason,
    string? TaskIdentifier,
    string ResponseBundleJson);
