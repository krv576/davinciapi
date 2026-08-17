using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Entities;
using DavinciEPA.Core.Enums;
using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Core.Results;
using DavinciEPA.Core.Services;
using FluentAssertions;
using Moq;

namespace PAS.Tests;

public class PriorAuthorizationServiceTests
{
    [Fact]
    public async Task SubmitAsync_WithInvalidBundle_ReturnsFhirValidationFailure()
    {
        var validatorMock = new Mock<IFhirResourceValidator>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<string>(), "Bundle", It.IsAny<string>()))
            .Returns(new FhirValidationOutcome(false, new[] { "Bundle.type is required." }, null));

        var service = new PriorAuthorizationService(
            Mock.Of<IUnitOfWork>(),
            validatorMock.Object,
            Mock.Of<IClaimResponseBuilder>(),
            Mock.Of<IBundleBuilder>(),
            Mock.Of<IPriorAuthorizationRuleEngine>());

        var result = await service.SubmitAsync(
            new SubmitPriorAuthorizationDto("patient-1", "payer-1", "70551", "claim-1", "{}"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.FhirValidation);
    }

    [Fact]
    public async Task SubmitAsync_WithValidBundleAndGrantedDecision_PersistsApprovedRequest()
    {
        var validatorMock = new Mock<IFhirResourceValidator>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<string>(), "Bundle", It.IsAny<string>()))
            .Returns(new FhirValidationOutcome(true, Array.Empty<string>(), null));

        var ruleEngineMock = new Mock<IPriorAuthorizationRuleEngine>();
        ruleEngineMock
            .Setup(e => e.EvaluateAsync(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PriorAuthorizationRuleDecision(PriorAuthorizationDisposition.Granted, "Auto-approved"));

        var claimResponseBuilderMock = new Mock<IClaimResponseBuilder>();
        claimResponseBuilderMock
            .Setup(b => b.BuildClaimResponseJson(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns("{\"resourceType\":\"ClaimResponse\"}");

        var bundleBuilderMock = new Mock<IBundleBuilder>();
        bundleBuilderMock
            .Setup(b => b.BuildCollectionBundleJson(It.IsAny<IReadOnlyCollection<string>>()))
            .Returns("{\"resourceType\":\"Bundle\"}");

        var requestRepoMock = new Mock<IPriorAuthorizationRequestRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.SetupGet(u => u.PriorAuthorizationRequests).Returns(requestRepoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new PriorAuthorizationService(
            unitOfWorkMock.Object,
            validatorMock.Object,
            claimResponseBuilderMock.Object,
            bundleBuilderMock.Object,
            ruleEngineMock.Object);

        var result = await service.SubmitAsync(
            new SubmitPriorAuthorizationDto("patient-1", "payer-1", "70551", "claim-1", "{\"resourceType\":\"Bundle\"}"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Disposition.Should().Be(PriorAuthorizationDisposition.Granted);
        requestRepoMock.Verify(r => r.AddAsync(It.IsAny<PriorAuthorizationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_ForAlreadyApprovedRequest_ReturnsConflict()
    {
        var entity = new PriorAuthorizationRequest(
            Guid.NewGuid(), "claim-1", "patient-1", "payer-1", "70551", DateTimeOffset.UtcNow);
        entity.MarkSubmitted();
        entity.RecordDecision(PriorAuthorizationDisposition.Granted, "Approved");

        var requestRepoMock = new Mock<IPriorAuthorizationRequestRepository>();
        requestRepoMock.Setup(r => r.GetByExternalIdAsync("claim-1", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.SetupGet(u => u.PriorAuthorizationRequests).Returns(requestRepoMock.Object);

        var service = new PriorAuthorizationService(
            unitOfWorkMock.Object,
            Mock.Of<IFhirResourceValidator>(),
            Mock.Of<IClaimResponseBuilder>(),
            Mock.Of<IBundleBuilder>(),
            Mock.Of<IPriorAuthorizationRuleEngine>());

        var result = await service.CancelAsync("claim-1", CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }
}
