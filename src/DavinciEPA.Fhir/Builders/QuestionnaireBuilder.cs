using DavinciEPA.Core.Constants;
using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Builders;

/// <summary>
/// Builds a DTR <c>Questionnaire</c> resource. Owns the catalog of known question sets keyed by canonical URL,
/// falling back to a general prior-authorization documentation template for unknown URLs.
/// </summary>
public sealed class QuestionnaireBuilder : IQuestionnaireBuilder
{
    private readonly FhirJsonSerializerService _serializerService;

    private static readonly IReadOnlyDictionary<string, (string LinkId, string Text, Questionnaire.QuestionnaireItemType Type)[]> ItemCatalog =
        new Dictionary<string, (string, string, Questionnaire.QuestionnaireItemType)[]>
        {
            ["default"] = new[]
            {
                ("clinical-indication", "What is the clinical indication for this service?", Questionnaire.QuestionnaireItemType.Text),
                ("diagnosis-code", "Primary diagnosis code (ICD-10-CM)", Questionnaire.QuestionnaireItemType.String),
                ("prior-treatments", "Describe prior treatments attempted and their outcomes.", Questionnaire.QuestionnaireItemType.Text),
                ("supporting-documentation", "Attach any supporting clinical documentation.", Questionnaire.QuestionnaireItemType.Attachment)
            }
        };

    public QuestionnaireBuilder(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public string BuildQuestionnaireJson(string canonicalUrl, string title)
    {
        var items = ItemCatalog.TryGetValue(canonicalUrl, out var catalogItems)
            ? catalogItems
            : ItemCatalog["default"];

        var questionnaire = new Questionnaire
        {
            Id = Guid.NewGuid().ToString("N"),
            Url = canonicalUrl,
            Meta = new Meta { Profile = new[] { DaVinciProfiles.DtrQuestionnaire } },
            Status = PublicationStatus.Active,
            Title = title,
            Date = DateTimeOffset.UtcNow.ToString("O")
        };

        foreach (var (linkId, text, type) in items)
        {
            questionnaire.Item.Add(new Questionnaire.ItemComponent
            {
                LinkId = linkId,
                Text = text,
                Type = type
            });
        }

        return _serializerService.Serialize(questionnaire);
    }
}
