using DavinciEPA.Core.Constants;
using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Entities;
using DavinciEPA.Core.Enums;
using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Core.Interfaces.Services;
using DavinciEPA.Core.Results;

namespace DavinciEPA.Core.Services;

/// <summary>
/// Orchestrates the prior authorization lifecycle for the PAS API: creation, $submit adjudication,
/// status lookup, $inquire polling, and cancellation. FHIR concerns are delegated to the injected ports
/// so this class never depends on the Firely SDK.
/// </summary>
public sealed class PriorAuthorizationService : IPriorAuthorizationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFhirResourceValidator _resourceValidator;
    private readonly IClaimResponseBuilder _claimResponseBuilder;
    private readonly IBundleBuilder _bundleBuilder;
    private readonly IPriorAuthorizationRuleEngine _ruleEngine;

    public PriorAuthorizationService(
        IUnitOfWork unitOfWork,
        IFhirResourceValidator resourceValidator,
        IClaimResponseBuilder claimResponseBuilder,
        IBundleBuilder bundleBuilder,
        IPriorAuthorizationRuleEngine ruleEngine)
    {
        _unitOfWork = unitOfWork;
        _resourceValidator = resourceValidator;
        _claimResponseBuilder = claimResponseBuilder;
        _bundleBuilder = bundleBuilder;
        _ruleEngine = ruleEngine;
    }

    public async Task<Result<PriorAuthorizationRequestDto>> CreateAsync(
        CreatePriorAuthorizationRequestDto request,
        CancellationToken cancellationToken)
    {
        var entity = new PriorAuthorizationRequest(
            Guid.NewGuid(),
            externalId: Guid.NewGuid().ToString("N"),
            request.PatientIdentifier,
            request.PayerId,
            request.OrderReference,
            DateTimeOffset.UtcNow);

        await _unitOfWork.PriorAuthorizationRequests.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(ToDto(entity));
    }

    public async Task<Result<PriorAuthorizationDecisionDto>> SubmitAsync(
        SubmitPriorAuthorizationDto request,
        CancellationToken cancellationToken)
    {
        var validation = _resourceValidator.Validate(request.RawBundleJson, "Bundle", DaVinciProfiles.PasBundle);
        if (!validation.IsValid)
        {
            return Result.Failure<PriorAuthorizationDecisionDto>(
                Error.FhirValidation("pas.bundle.invalid", string.Join("; ", validation.Issues)));
        }

        var entity = new PriorAuthorizationRequest(
            Guid.NewGuid(),
            request.ClaimReference,
            request.PatientIdentifier,
            request.PayerId,
            request.OrderReference,
            DateTimeOffset.UtcNow);

        entity.MarkSubmitted();

        var decision = await _ruleEngine.EvaluateAsync(request.RawBundleJson, Array.Empty<string>(), cancellationToken);

        string? taskIdentifier = null;
        if (decision.Disposition == PriorAuthorizationDisposition.Pending)
        {
            taskIdentifier = Guid.NewGuid().ToString("N");
            entity.MarkPended(taskIdentifier);
        }
        else
        {
            entity.RecordDecision(decision.Disposition, decision.Reason);
        }

        await _unitOfWork.PriorAuthorizationRequests.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responseBundleJson = BuildResponseBundle(
            request.ClaimReference,
            request.PatientIdentifier,
            request.PayerId,
            decision,
            taskIdentifier);

        return Result.Success(new PriorAuthorizationDecisionDto(
            entity.Id,
            decision.Disposition,
            decision.Reason,
            taskIdentifier,
            responseBundleJson));
    }

    public async Task<Result<PriorAuthorizationRequestDto>> GetStatusAsync(
        string externalId,
        CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.PriorAuthorizationRequests.GetByExternalIdAsync(externalId, cancellationToken);
        return entity is null
            ? Result.Failure<PriorAuthorizationRequestDto>(
                Error.NotFound("pas.request.not-found", $"No prior authorization request found for '{externalId}'."))
            : Result.Success(ToDto(entity));
    }

    public async Task<Result<PriorAuthorizationDecisionDto>> InquireAsync(
        string externalId,
        CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.PriorAuthorizationRequests.GetByExternalIdAsync(externalId, cancellationToken);
        if (entity is null)
        {
            return Result.Failure<PriorAuthorizationDecisionDto>(
                Error.NotFound("pas.request.not-found", $"No prior authorization request found for '{externalId}'."));
        }

        var outcome = entity.Disposition switch
        {
            PriorAuthorizationDisposition.Granted => "complete",
            PriorAuthorizationDisposition.PartiallyGranted => "complete",
            PriorAuthorizationDisposition.Denied => "complete",
            PriorAuthorizationDisposition.Cancelled => "complete",
            _ => "queued"
        };

        var claimResponseJson = _claimResponseBuilder.BuildClaimResponseJson(
            entity.ExternalId,
            entity.PatientIdentifier,
            entity.PayerId,
            outcome,
            entity.Disposition?.ToString(),
            entity.TaskIdentifier);

        var responseBundleJson = _bundleBuilder.BuildCollectionBundleJson(new[] { claimResponseJson });

        return Result.Success(new PriorAuthorizationDecisionDto(
            entity.Id,
            entity.Disposition ?? PriorAuthorizationDisposition.Pending,
            entity.DispositionReason,
            entity.TaskIdentifier,
            responseBundleJson));
    }

    public async Task<Result<PriorAuthorizationRequestDto>> CancelAsync(
        string externalId,
        CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.PriorAuthorizationRequests.GetByExternalIdAsync(externalId, cancellationToken);
        if (entity is null)
        {
            return Result.Failure<PriorAuthorizationRequestDto>(
                Error.NotFound("pas.request.not-found", $"No prior authorization request found for '{externalId}'."));
        }

        if (entity.Status is PriorAuthorizationStatus.Approved or PriorAuthorizationStatus.Denied or PriorAuthorizationStatus.Cancelled)
        {
            return Result.Failure<PriorAuthorizationRequestDto>(
                Error.Conflict("pas.request.terminal", $"Request '{externalId}' is already in a terminal state ({entity.Status})."));
        }

        entity.RecordDecision(PriorAuthorizationDisposition.Cancelled, "Cancelled by requester.");
        _unitOfWork.PriorAuthorizationRequests.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(ToDto(entity));
    }

    private string BuildResponseBundle(
        string claimReference,
        string patientIdentifier,
        string payerId,
        PriorAuthorizationRuleDecision decision,
        string? taskIdentifier)
    {
        var outcome = decision.Disposition == PriorAuthorizationDisposition.Pending ? "queued" : "complete";
        var claimResponseJson = _claimResponseBuilder.BuildClaimResponseJson(
            claimReference,
            patientIdentifier,
            payerId,
            outcome,
            decision.Disposition.ToString(),
            taskIdentifier);

        return _bundleBuilder.BuildCollectionBundleJson(new[] { claimResponseJson });
    }

    private static PriorAuthorizationRequestDto ToDto(PriorAuthorizationRequest entity) => new(
        entity.Id,
        entity.ExternalId,
        entity.PatientIdentifier,
        entity.PayerId,
        entity.OrderReference,
        entity.Status,
        entity.Disposition,
        entity.DispositionReason,
        entity.TaskIdentifier,
        entity.CreatedAt,
        entity.UpdatedAt);
}
