using DavinciEPA.Core.Enums;

namespace DavinciEPA.Core.DTOs;

/// <summary>A DTR Questionnaire package (Questionnaire plus any pre-population context) returned to the SMART app.</summary>
public sealed record QuestionnairePackageDto(
    string CanonicalUrl,
    string QuestionnaireResourceJson,
    IReadOnlyDictionary<string, string> PrepopulatedAnswers);

/// <summary>A completed QuestionnaireResponse submitted back from the DTR SMART app.</summary>
public sealed record SubmitQuestionnaireResponseDto(
    Guid DocumentationRequirementId,
    string QuestionnaireResponseReference,
    string QuestionnaireResponseResourceJson);

/// <summary>Result of accepting/validating a submitted QuestionnaireResponse.</summary>
public sealed record QuestionnaireResponseResultDto(
    Guid DocumentationRequirementId,
    DocumentationRequirementStatus Status,
    string QuestionnaireResponseReference);
