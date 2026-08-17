namespace DavinciEPA.Rules.MedicalNecessity;

/// <summary>Ties a coverage requirement to the diagnosis codes that satisfy its medical necessity criteria.</summary>
public sealed record MedicalNecessityCriterion(string RequirementCode, IReadOnlyCollection<string> QualifyingConditionCodes);

/// <summary>Catalog of clinical criteria backing each coverage requirement rule.</summary>
public static class MedicalNecessityCriteriaCatalog
{
    public static readonly IReadOnlyCollection<MedicalNecessityCriterion> Criteria = new[]
    {
        new MedicalNecessityCriterion("PA-IMAGING-ADVANCED", new[] { "M54.5", "G89.29", "R51" }),
        new MedicalNecessityCriterion("PA-DME-POWER-WHEELCHAIR", new[] { "G80.9", "M62.81", "G82.20" })
    };
}
