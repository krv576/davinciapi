namespace DavinciEPA.Core.Constants;

/// <summary>Canonical StructureDefinition profile URLs for the Da Vinci CRD, DTR, and PAS Implementation Guides.</summary>
public static class DaVinciProfiles
{
    // Da Vinci CRD (Coverage Requirements Discovery)
    public const string CrdCoverage = "http://hl7.org/fhir/us/davinci-crd/StructureDefinition/profile-coverage";
    public const string CrdServiceRequest = "http://hl7.org/fhir/us/davinci-crd/StructureDefinition/profile-servicerequest";
    public const string CrdDeviceRequest = "http://hl7.org/fhir/us/davinci-crd/StructureDefinition/profile-devicerequest";
    public const string CrdMedicationRequest = "http://hl7.org/fhir/us/davinci-crd/StructureDefinition/profile-medicationrequest";

    // Da Vinci DTR (Documentation Templates and Rules)
    public const string DtrQuestionnaire = "http://hl7.org/fhir/us/davinci-dtr/StructureDefinition/dtr-questionnaire";
    public const string DtrQuestionnaireResponse = "http://hl7.org/fhir/us/davinci-dtr/StructureDefinition/dtr-questionnaireresponse";
    public const string DtrQuestionnaireAdapt = "http://hl7.org/fhir/us/davinci-dtr/StructureDefinition/dtr-adapt-questionnaire";

    // Da Vinci PAS (Prior Authorization Support)
    public const string PasClaim = "http://hl7.org/fhir/us/davinci-pas/StructureDefinition/profile-claim";
    public const string PasClaimResponse = "http://hl7.org/fhir/us/davinci-pas/StructureDefinition/profile-claimresponse";
    public const string PasBundle = "http://hl7.org/fhir/us/davinci-pas/StructureDefinition/profile-bundle";
    public const string PasTask = "http://hl7.org/fhir/us/davinci-pas/StructureDefinition/profile-task";
    public const string PasCoverage = "http://hl7.org/fhir/us/davinci-pas/StructureDefinition/profile-coverage";

    // US Core context resources referenced across all three IGs
    public const string UsCorePatient = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient";
    public const string UsCorePractitioner = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-practitioner";
    public const string UsCoreOrganization = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-organization";
    public const string UsCoreEncounter = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-encounter";
    public const string UsCoreCondition = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-condition";
    public const string UsCoreObservation = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-observation-lab";
}

/// <summary>Code system URIs used for coded values exchanged across the Da Vinci EPA workflow.</summary>
public static class FhirCodeSystems
{
    public const string Icd10Cm = "http://hl7.org/fhir/sid/icd-10-cm";
    public const string Cpt = "http://www.ama-assn.org/go/cpt";
    public const string Hcpcs = "https://www.cms.gov/Medicare/Coding/HCPCSReleaseCodeSets";
    public const string ClaimAdjustmentReasonCodes = "https://x12.org/codes/claim-adjustment-reason-codes";
    public const string ClaimUse = "http://hl7.org/fhir/fm-status";
    public const string ProcessPriority = "http://terminology.hl7.org/CodeSystem/processpriority";
    public const string ClaimType = "http://terminology.hl7.org/CodeSystem/claim-type";
    public const string ClaimSubType = "http://terminology.hl7.org/CodeSystem/ex-claimsubtype";
    public const string Npi = "http://hl7.org/fhir/sid/us-npi";
}

/// <summary>CDS Hooks protocol constants used by the CRD service.</summary>
public static class CdsHooksConstants
{
    public const string OrderSelectHook = "order-select";
    public const string OrderSignHook = "order-sign";
    public const string ApptBookHook = "appointment-book";

    public const string CardIndicatorInfo = "info";
    public const string CardIndicatorWarning = "warning";
    public const string CardIndicatorCritical = "critical";

    public const string SmartLinkType = "smart";
    public const string AbsoluteLinkType = "absolute";

    public const string ContentType = "application/json";
}

/// <summary>SMART App Launch / SMART on FHIR scope and parameter constants used by the DTR service.</summary>
public static class SmartOnFhirConstants
{
    public const string LaunchParam = "launch";
    public const string IssParam = "iss";
    public const string PatientContextParam = "patient";
    public const string EncounterContextParam = "encounter";

    public const string OpenIdScope = "openid";
    public const string FhirUserScope = "fhirUser";
    public const string LaunchScope = "launch";
    public const string OfflineAccessScope = "offline_access";
    public const string PatientReadAllScope = "patient/*.read";

    public const string GrantTypeAuthorizationCode = "authorization_code";
    public const string GrantTypeClientCredentials = "client_credentials";
    public const string CodeChallengeMethodS256 = "S256";
}
