using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Results;

namespace DavinciEPA.Core.Interfaces.Services;

/// <summary>Application service implementing DTR questionnaire retrieval, pre-population, and response submission.</summary>
public interface IDocumentationRequirementService
{
    Task<Result<QuestionnairePackageDto>> GetQuestionnairePackageAsync(
        Guid documentationRequirementId,
        string patientFhirId,
        string fhirServerBaseUrl,
        string? accessToken,
        CancellationToken cancellationToken);

    Task<Result<QuestionnaireResponseResultDto>> SubmitResponseAsync(
        SubmitQuestionnaireResponseDto request,
        CancellationToken cancellationToken);
}
