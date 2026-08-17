---
applyTo: "src/DavinciEPA.Fhir/**,src/DavinciEPA.*.Api/**"
---

# FHIR & Da Vinci IG Instructions

Applies to FHIR resource handling in `DavinciEPA.Fhir` and any API project that produces/consumes FHIR resources.

## Library & version

- FHIR version: **R4** (per CRD, DTR, and PAS IGs).
- Use the **Firely .NET SDK** (`Hl7.Fhir.R4` / `Hl7.Fhir.Support` NuGet packages) for all resource POCOs, parsing, and serialization. Do not hand-construct FHIR JSON/XML with `JsonDocument`/anonymous objects.
- Serialize with the SDK's `FhirJsonSerializer`/`FhirJsonParser` (or the `System.Text.Json` extensions the SDK provides), configured for `Pretty = false` in production responses.

## Resource construction

- All resource-building logic (mapping domain models → FHIR resources and back) lives in `DavinciEPA.Fhir`, behind an interface consumed by `Core`/`*.Api` — never build resources inline in a controller.
- Every resource must set `Meta.Profile` to the canonical URL of the Da Vinci profile it conforms to (see [docs/fhir-resources.md](../../docs/fhir-resources.md) for the list per IG).
- Populate mandatory (`1..1`) elements required by the profile; do not leave required `Coding`/`CodeableConcept` elements null just to make code compile.
- Use `Bundle` of type `transaction` or `searchset` as required by the interaction (PAS `$submit` uses a `collection` Bundle; search results use `searchset`).

## Validation

- Validate constructed resources against the applicable StructureDefinition/profile before returning them from an API (use the SDK's `Validator`/`IResourceValidator`, or an external FHIR validator in CI) — do not skip validation "for now" on IG-critical resources (`Claim`, `ClaimResponse`, `QuestionnaireResponse`, `Bundle`).
- On validation failure, return a FHIR `OperationOutcome` with actionable `issue.diagnostics`, not a generic 500.

## Per-IG specifics

- **CRD**: implement as a CDS Hooks service (`hook`, `hookInstance`, `context`, `prefetch`) — responses are CDS Hooks `Card` JSON, not raw FHIR resources, except for embedded `CoverageRequirements` links.
- **DTR**: `Questionnaire`/`QuestionnaireResponse` must reference the triggering order (`ServiceRequest`/`DeviceRequest`/etc.) via `QuestionnaireResponse.subject`/context extensions defined by the DTR IG. Launch context follows SMART App Launch.
- **PAS**: `Claim` (use = `preauthorization`) and `ClaimResponse` exchange happens via the `$submit` operation on a `Bundle`; asynchronous responses use `$inquire` and Task-based polling as defined by the PAS IG.

## Terminology

- Prefer coded values from required/extensible bindings named in each profile (e.g. `X12 Claim Adjustment Reason Codes`, CPT/HCPCS, ICD-10-CM) over free text — store the code system URI alongside the code.
