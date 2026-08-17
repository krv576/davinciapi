namespace DavinciEPA.DTR.Api.Configuration;

/// <summary>This DTR app's own SMART on FHIR client registration details.</summary>
public sealed class SmartClientOptions
{
    public const string SectionName = "Smart";

    public string ClientId { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;

    public string Scope { get; set; } = "launch openid fhirUser patient/*.read";
}
