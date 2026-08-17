namespace DavinciEPA.Rules.Coverage;

/// <summary>A single data-driven coverage requirement rule: which procedure/product codes trigger it and what documentation it requires.</summary>
public sealed record CoverageRuleDefinition(
    string RequirementCode,
    string Description,
    IReadOnlyCollection<string> ApplicableProcedureCodes,
    string DocumentationQuestionnaireCanonicalUrl);

/// <summary>
/// The catalog of known payer coverage rules. In a production deployment this would be sourced from
/// per-payer configuration (e.g. persisted and managed via an admin workflow); it is expressed here as a
/// typed, in-memory catalog so the evaluation logic itself is fully real and testable.
/// </summary>
public static class CoverageRuleCatalog
{
    public static readonly IReadOnlyCollection<CoverageRuleDefinition> Rules = new[]
    {
        new CoverageRuleDefinition(
            RequirementCode: "PA-IMAGING-ADVANCED",
            Description: "Advanced imaging services (MRI/CT) require prior authorization and supporting clinical documentation.",
            ApplicableProcedureCodes: new[] { "70551", "70552", "70553", "74176", "74177", "74178" },
            DocumentationQuestionnaireCanonicalUrl: "http://davinciepa.local/fhir/Questionnaire/advanced-imaging-dtr"),

        new CoverageRuleDefinition(
            RequirementCode: "PA-DME-POWER-WHEELCHAIR",
            Description: "Power mobility devices require prior authorization and medical necessity documentation.",
            ApplicableProcedureCodes: new[] { "K0813", "K0814", "K0815", "K0816" },
            DocumentationQuestionnaireCanonicalUrl: "http://davinciepa.local/fhir/Questionnaire/dme-mobility-dtr")
    };
}
