using DavinciEPA.Core.Interfaces.External;
using DavinciEPA.Fhir.Serialization;
using DavinciEPA.Rules.Documentation;
using FluentAssertions;
using Hl7.Fhir.Model;
using Moq;
using Task = System.Threading.Tasks.Task;

namespace DTR.Tests;

public class QuestionnairePrePopulationEngineTests
{
    [Fact]
    public async Task PrepopulateAsync_WithActiveCondition_PopulatesDiagnosisCodeAndClinicalIndication()
    {
        var serializerService = new FhirJsonSerializerService();
        var condition = new Condition
        {
            Code = new CodeableConcept("http://hl7.org/fhir/sid/icd-10-cm", "M54.5") { Text = "Low back pain" }
        };
        var bundle = new Bundle { Type = Bundle.BundleType.Searchset };
        bundle.Entry.Add(new Bundle.EntryComponent { Resource = condition });
        var bundleJson = serializerService.Serialize(bundle);

        var ehrClientMock = new Mock<IEhrFhirClient>();
        ehrClientMock
            .Setup(c => c.SearchAsync(
                It.IsAny<string>(),
                It.IsAny<string?>(),
                "Condition",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(bundleJson);

        var engine = new QuestionnairePrePopulationEngine(ehrClientMock.Object, serializerService);

        var answers = await engine.PrepopulateAsync(
            "http://example.org/Questionnaire/1",
            "patient-1",
            "https://ehr.example.org/fhir",
            "token",
            CancellationToken.None);

        answers["diagnosis-code"].Should().Be("M54.5");
        answers["clinical-indication"].Should().Be("Low back pain");
    }

    [Fact]
    public async Task PrepopulateAsync_WithNoConditionsFound_ReturnsEmptyAnswers()
    {
        var serializerService = new FhirJsonSerializerService();
        var ehrClientMock = new Mock<IEhrFhirClient>();
        ehrClientMock
            .Setup(c => c.SearchAsync(
                It.IsAny<string>(),
                It.IsAny<string?>(),
                "Condition",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var engine = new QuestionnairePrePopulationEngine(ehrClientMock.Object, serializerService);

        var answers = await engine.PrepopulateAsync(
            "http://example.org/Questionnaire/1",
            "patient-1",
            "https://ehr.example.org/fhir",
            null,
            CancellationToken.None);

        answers.Should().BeEmpty();
    }
}
