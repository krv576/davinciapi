# FHIR Resources

FHIR version: **R4**. Resource handling lives exclusively in `DavinciEPA.Fhir`, built on the Firely .NET SDK (`Hl7.Fhir.R4`). See [fhir.instructions.md](../.github/instructions/fhir.instructions.md) for construction/validation rules.

## CRD (Coverage Requirements Discovery)

Protocol: CDS Hooks (not raw FHIR REST) — payloads are CDS Hooks JSON, but reference/embed FHIR resources.

| Resource | Purpose |
|---|---|
| `ServiceRequest` / `DeviceRequest` / `MedicationRequest` | The order in context that triggers coverage requirement discovery (via `hook` context/prefetch). |
| `Coverage` | Patient's payer coverage, used to determine which payer's rules apply. |
| `Patient`, `Encounter`, `Practitioner` | Context resources typically supplied via `prefetch`. |
| CDS Hooks `Card` | Not a FHIR resource — the CRD response shape carrying `summary`/`indicator`/`detail`/`links` (including a `smart` link to the DTR app). |

## DTR (Documentation Templates and Rules)

Protocol: SMART on FHIR app backed by FHIR REST.

| Resource | Purpose |
|---|---|
| `Questionnaire` | The documentation template returned to the DTR app; may embed pre-population rules (CQL/FHIRPath expressions per the DTR IG extensions). |
| `QuestionnaireResponse` | The clinician/patient-completed answers, submitted back to DTR and forwarded toward PAS. |
| `Patient`, `Encounter`, `Condition`, `Observation`, `MedicationRequest`, etc. | Source data queried from the EHR's FHIR server to pre-populate questionnaire answers. |

## PAS (Prior Authorization Support)

Protocol: FHIR REST operations (`$submit`, `$inquire`).

| Resource | Purpose |
|---|---|
| `Bundle` (type `collection`, then wrapped for `$submit`) | Carries the `Claim` plus supporting resources (`QuestionnaireResponse`, `Patient`, `Coverage`, clinical evidence) in a single submission. |
| `Claim` (`use = preauthorization`) | The prior authorization request itself. |
| `ClaimResponse` | The payer's decision: `outcome` of `complete`, `error`, or `queued`, with `adjudication`/`item` detail on approval/denial. |
| `Task` | Used for asynchronous status tracking when a decision is `queued` (`$inquire` flow). |
| `Coverage`, `Patient`, `Organization`, `Practitioner` | Supporting context resources referenced from `Claim`. |

## Profile conformance

Every resource built or accepted by this system must declare `Meta.Profile` with the canonical URL of the applicable Da Vinci profile (CRD, DTR, or PAS Implementation Guide, as published by HL7) and satisfy that profile's cardinality/binding constraints. When adding support for a new resource:

1. Confirm which profile (CRD/DTR/PAS) it belongs to and record the canonical profile URL here.
2. Implement the builder/parser in `DavinciEPA.Fhir` per [fhir.instructions.md](../.github/instructions/fhir.instructions.md).
3. Add a validation test asserting the constructed resource conforms (mandatory elements present, correct `Meta.Profile`).

## Terminology bindings

Prefer standard code systems required/suggested by each IG over local codes:

- Diagnoses: ICD-10-CM.
- Procedures/services: CPT / HCPCS.
- Claim adjustment reasons: X12 Claim Adjustment Reason Codes.
- Coverage/payer identifiers: NPI (providers/organizations), payer-assigned member IDs.

Store both the `system` URI and `code` together; do not persist bare codes without their code system.
