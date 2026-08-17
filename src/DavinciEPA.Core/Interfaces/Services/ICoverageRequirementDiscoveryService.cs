using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Results;

namespace DavinciEPA.Core.Interfaces.Services;

/// <summary>Application service implementing Coverage Requirements Discovery for a CDS Hooks invocation.</summary>
public interface ICoverageRequirementDiscoveryService
{
    Task<Result<CoverageRequirementDiscoveryResultDto>> DiscoverAsync(
        CoverageRequirementDiscoveryRequestDto request,
        CancellationToken cancellationToken);
}
