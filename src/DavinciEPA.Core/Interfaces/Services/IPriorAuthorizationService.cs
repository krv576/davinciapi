using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Results;

namespace DavinciEPA.Core.Interfaces.Services;

/// <summary>Application service orchestrating the prior authorization lifecycle (submission through decision).</summary>
public interface IPriorAuthorizationService
{
    Task<Result<PriorAuthorizationRequestDto>> CreateAsync(
        CreatePriorAuthorizationRequestDto request,
        CancellationToken cancellationToken);

    Task<Result<PriorAuthorizationDecisionDto>> SubmitAsync(
        SubmitPriorAuthorizationDto request,
        CancellationToken cancellationToken);

    Task<Result<PriorAuthorizationRequestDto>> GetStatusAsync(string externalId, CancellationToken cancellationToken);

    Task<Result<PriorAuthorizationDecisionDto>> InquireAsync(string externalId, CancellationToken cancellationToken);

    Task<Result<PriorAuthorizationRequestDto>> CancelAsync(string externalId, CancellationToken cancellationToken);
}
