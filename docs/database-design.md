# Database Design

Persistence lives entirely in `DavinciEPA.Infrastructure` via EF Core, behind repository interfaces defined in `DavinciEPA.Core`. See [database.instructions.md](../.github/instructions/database.instructions.md) for conventions.

> This is the target entity model for the persistence layer. Adjust this document alongside the initial migration as the schema is implemented — keep it in sync with reality rather than aspirational.

## Core entities

### `PriorAuthRequest`
Tracks a prior authorization request end-to-end across CRD → DTR → PAS.

| Column | Type | Notes |
|---|---|---|
| `Id` | `Guid` (PK) | Internal identifier. |
| `ExternalId` | `string` | Correlates to the FHIR `Claim.id`/business identifier submitted to PAS. |
| `PatientIdentifier` | `string` | Synthetic/business patient identifier (not PHI-bearing internal key where avoidable). |
| `PayerId` | `string` | Identifies the payer whose rules/adjudication apply. |
| `Status` | `enum` | `Draft`, `DocumentationRequired`, `Submitted`, `Pended`, `Approved`, `Denied`, `Error`. |
| `CreatedAt` / `UpdatedAt` | `DateTimeOffset` | Audit timestamps. |

### `CoverageRequirement`
Result of a CRD rule evaluation for a given order.

| Column | Type | Notes |
|---|---|---|
| `Id` | `Guid` (PK) | |
| `PriorAuthRequestId` | `Guid` (FK, nullable) | Set once a request is created; may exist standalone for CRD-only evaluations. |
| `OrderReference` | `string` | FHIR reference to the triggering order (`ServiceRequest`/`DeviceRequest`/etc.). |
| `RequirementCode` | `string` | Identifier for the specific coverage rule evaluated. |
| `IsMet` | `bool` | Whether the requirement is satisfied by available data. |
| `EvaluatedAt` | `DateTimeOffset` | |

### `DocumentationRequirement`
Tracks DTR questionnaire completion for a request.

| Column | Type | Notes |
|---|---|---|
| `Id` | `Guid` (PK) | |
| `PriorAuthRequestId` | `Guid` (FK) | |
| `QuestionnaireCanonicalUrl` | `string` | Which `Questionnaire` applies. |
| `QuestionnaireResponseReference` | `string` | Reference/ID of the stored `QuestionnaireResponse` (raw resource retained in `Infrastructure`'s FHIR store or as JSON below). |
| `Status` | `enum` | `Pending`, `InProgress`, `Completed`. |

### `RuleEvaluationLog`
Auditable record of every rule evaluation (coverage or pre-population), independent of outcome storage above.

| Column | Type | Notes |
|---|---|---|
| `Id` | `Guid` (PK) | |
| `PriorAuthRequestId` | `Guid` (FK, nullable) | |
| `RuleId` | `string` | |
| `InputSummary` | `string` (JSON, no PHI beyond necessary coded values) | |
| `Result` | `string` | Serialized outcome. |
| `EvaluatedAt` | `DateTimeOffset` | |

### `AuditLog`
System-wide audit trail (who/what/when) for compliance.

| Column | Type | Notes |
|---|---|---|
| `Id` | `Guid` (PK) | |
| `ActorId` | `string` | Client/user identifier from the validated token. |
| `Action` | `string` | e.g. `Claim.Submit`, `QuestionnaireResponse.Submit`. |
| `ResourceReference` | `string` | What was acted on. |
| `Timestamp` | `DateTimeOffset` | |

## Indexing

- `PriorAuthRequest.ExternalId` — unique index (lookup by payer/EHR-facing identifier).
- `PriorAuthRequest.PatientIdentifier`, `PriorAuthRequest.Status` — non-unique indexes for status/patient queries.
- `CoverageRequirement.PriorAuthRequestId`, `DocumentationRequirement.PriorAuthRequestId` — FK indexes.

## Raw resource retention

Where the raw FHIR resource (e.g. submitted `Bundle`, generated `ClaimResponse`) must be retained for audit/replay, store it as a JSON column (or in a `RawResources` table keyed by `PriorAuthRequestId` + `ResourceType`) alongside the structured columns above — do not rely on JSON blobs as the only queryable representation of operational state.

## Migrations

Generate and commit migrations from `DavinciEPA.Infrastructure` (see [build-infrastructure.prompt.md](../.github/prompts/build-infrastructure.prompt.md)):

```bash
dotnet ef migrations add InitialCreate --project src/DavinciEPA.Infrastructure --startup-project src/DavinciEPA.PAS.Api
dotnet ef database update --project src/DavinciEPA.Infrastructure --startup-project src/DavinciEPA.PAS.Api
```
