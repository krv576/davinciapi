using DavinciEPA.Core.Entities;
using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Core.Results;
using DavinciEPA.Core.Services;
using FluentAssertions;
using Moq;

namespace DTR.Tests;

public class DocumentationRequirementServiceTests
{
    [Fact]
    public async Task GetQuestionnairePackageAsync_ForUnknownId_ReturnsNotFound()
    {
        var docRepoMock = new Mock<IDocumentationRequirementRepository>();
        docRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DocumentationRequirement?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.SetupGet(u => u.DocumentationRequirements).Returns(docRepoMock.Object);

        var service = new DocumentationRequirementService(
            unitOfWorkMock.Object,
            Mock.Of<IQuestionnaireBuilder>(),
            Mock.Of<IQuestionnairePrePopulationEngine>(),
            Mock.Of<IFhirResourceValidator>());

        var result = await service.GetQuestionnairePackageAsync(
            Guid.NewGuid(), "patient-1", "https://ehr.example.org/fhir", null, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task SubmitResponseAsync_WithInvalidQuestionnaireResponse_ReturnsFhirValidationFailure()
    {
        var requirement = new DocumentationRequirement(
            Guid.NewGuid(), Guid.NewGuid(), "http://example.org/Questionnaire/1", DateTimeOffset.UtcNow);

        var docRepoMock = new Mock<IDocumentationRequirementRepository>();
        docRepoMock
            .Setup(r => r.GetByIdAsync(requirement.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(requirement);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.SetupGet(u => u.DocumentationRequirements).Returns(docRepoMock.Object);

        var validatorMock = new Mock<IFhirResourceValidator>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<string>(), "QuestionnaireResponse", It.IsAny<string>()))
            .Returns(new FhirValidationOutcome(false, new[] { "QuestionnaireResponse.status is required." }, null));

        var service = new DocumentationRequirementService(
            unitOfWorkMock.Object,
            Mock.Of<IQuestionnaireBuilder>(),
            Mock.Of<IQuestionnairePrePopulationEngine>(),
            validatorMock.Object);

        var result = await service.SubmitResponseAsync(
            new DavinciEPA.Core.DTOs.SubmitQuestionnaireResponseDto(requirement.Id, "QuestionnaireResponse/1", "{}"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.FhirValidation);
    }
}
