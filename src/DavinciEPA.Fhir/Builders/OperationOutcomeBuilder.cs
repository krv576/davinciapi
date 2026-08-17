using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Builders;

/// <summary>Builds a FHIR <c>OperationOutcome</c> resource for validation/processing failures, per the Da Vinci error-handling convention.</summary>
public sealed class OperationOutcomeBuilder : IOperationOutcomeBuilder
{
    private readonly FhirJsonSerializerService _serializerService;

    public OperationOutcomeBuilder(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public string BuildErrorJson(string diagnostics, IReadOnlyCollection<string>? issues = null)
    {
        var outcome = new OperationOutcome();

        if (issues is null || issues.Count == 0)
        {
            outcome.Issue.Add(new OperationOutcome.IssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Error,
                Code = OperationOutcome.IssueType.Processing,
                Diagnostics = diagnostics
            });
        }
        else
        {
            foreach (var issue in issues)
            {
                outcome.Issue.Add(new OperationOutcome.IssueComponent
                {
                    Severity = OperationOutcome.IssueSeverity.Error,
                    Code = OperationOutcome.IssueType.Invalid,
                    Diagnostics = issue
                });
            }
        }

        return _serializerService.Serialize(outcome);
    }

    public string BuildFromValidationFailuresJson(IReadOnlyCollection<string> failures) =>
        BuildErrorJson("One or more FHIR validation issues were found.", failures);
}
