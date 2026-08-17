using DavinciEPA.Core.Constants;
using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Core.Interfaces.Services;
using DavinciEPA.Core.Results;

namespace DavinciEPA.Core.Services;

/// <summary>
/// Implements the DTR workflow: retrieving a pre-populated Questionnaire package for a documentation
/// requirement and accepting/validating the resulting QuestionnaireResponse.
/// </summary>
public sealed class DocumentationRequirementService : IDocumentationRequirementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuestionnaireBuilder _questionnaireBuilder;
    private readonly IQuestionnairePrePopulationEngine _prePopulationEngine;
    private readonly IFhirResourceValidator _resourceValidator;

    public DocumentationRequirementService(
        IUnitOfWork unitOfWork,
        IQuestionnaireBuilder questionnaireBuilder,
        IQuestionnairePrePopulationEngine prePopulationEngine,
        IFhirResourceValidator resourceValidator)
    {
        _unitOfWork = unitOfWork;
        _questionnaireBuilder = questionnaireBuilder;
        _prePopulationEngine = prePopulationEngine;
        _resourceValidator = resourceValidator;
    }

    public async Task<Result<QuestionnairePackageDto>> GetQuestionnairePackageAsync(
        Guid documentationRequirementId,
        string patientFhirId,
        string fhirServerBaseUrl,
        string? accessToken,
        CancellationToken cancellationToken)
    {
        var requirement = await _unitOfWork.DocumentationRequirements.GetByIdAsync(documentationRequirementId, cancellationToken);
        if (requirement is null)
        {
            return Result.Failure<QuestionnairePackageDto>(
                Error.NotFound("dtr.requirement.not-found", $"Documentation requirement '{documentationRequirementId}' was not found."));
        }

        requirement.MarkInProgress();
        _unitOfWork.DocumentationRequirements.Update(requirement);

        var questionnaireJson = _questionnaireBuilder.BuildQuestionnaireJson(
            requirement.QuestionnaireCanonicalUrl,
            title: "Prior Authorization Supporting Documentation");

        var prepopulated = await _prePopulationEngine.PrepopulateAsync(
            requirement.QuestionnaireCanonicalUrl,
            patientFhirId,
            fhirServerBaseUrl,
            accessToken,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new QuestionnairePackageDto(
            requirement.QuestionnaireCanonicalUrl,
            questionnaireJson,
            prepopulated));
    }

    public async Task<Result<QuestionnaireResponseResultDto>> SubmitResponseAsync(
        SubmitQuestionnaireResponseDto request,
        CancellationToken cancellationToken)
    {
        var requirement = await _unitOfWork.DocumentationRequirements.GetByIdAsync(
            request.DocumentationRequirementId,
            cancellationToken);

        if (requirement is null)
        {
            return Result.Failure<QuestionnaireResponseResultDto>(
                Error.NotFound(
                    "dtr.requirement.not-found",
                    $"Documentation requirement '{request.DocumentationRequirementId}' was not found."));
        }

        var validation = _resourceValidator.Validate(
            request.QuestionnaireResponseResourceJson,
            "QuestionnaireResponse",
            DaVinciProfiles.DtrQuestionnaireResponse);

        if (!validation.IsValid)
        {
            return Result.Failure<QuestionnaireResponseResultDto>(
                Error.FhirValidation("dtr.questionnaireresponse.invalid", string.Join("; ", validation.Issues)));
        }

        requirement.Complete(request.QuestionnaireResponseReference);
        _unitOfWork.DocumentationRequirements.Update(requirement);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new QuestionnaireResponseResultDto(
            requirement.Id,
            requirement.Status,
            requirement.QuestionnaireResponseReference!));
    }
}
