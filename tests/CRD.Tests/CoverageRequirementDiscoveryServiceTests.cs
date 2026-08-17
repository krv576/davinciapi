using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Entities;
using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Core.Services;
using FluentAssertions;
using Moq;

namespace CRD.Tests;

public class CoverageRequirementDiscoveryServiceTests
{
    [Fact]
    public async Task DiscoverAsync_PersistsEvaluationsAndAuditLogsThenReturnsResults()
    {
        var expected = new List<CoverageRequirementResultDto>
        {
            new("PA-IMAGING-ADVANCED", "Advanced imaging requires prior authorization.", false, "http://example.org/Questionnaire/1")
        };

        var ruleEngineMock = new Mock<ICoverageRuleEngine>();
        ruleEngineMock
            .Setup(e => e.EvaluateAsync(It.IsAny<CoverageRequirementDiscoveryRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var coverageRepoMock = new Mock<ICoverageRequirementRepository>();
        var ruleLogRepoMock = new Mock<IRuleEvaluationLogRepository>();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.SetupGet(u => u.CoverageRequirements).Returns(coverageRepoMock.Object);
        unitOfWorkMock.SetupGet(u => u.RuleEvaluationLogs).Returns(ruleLogRepoMock.Object);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new CoverageRequirementDiscoveryService(ruleEngineMock.Object, unitOfWorkMock.Object);
        var request = new CoverageRequirementDiscoveryRequestDto(
            "order-select", "patient-1", "payer-1", "order-1", "{}", new Dictionary<string, string>());

        var result = await service.DiscoverAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Requirements.Should().BeEquivalentTo(expected);
        coverageRepoMock.Verify(r => r.AddAsync(It.IsAny<CoverageRequirementEvaluation>(), It.IsAny<CancellationToken>()), Times.Once);
        ruleLogRepoMock.Verify(r => r.AddAsync(It.IsAny<RuleEvaluationLog>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
