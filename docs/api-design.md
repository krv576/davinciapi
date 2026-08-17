# API Design

Conventions for the three HTTP APIs (`DavinciEPA.CRD.Api`, `DavinciEPA.DTR.Api`, `DavinciEPA.PAS.Api`). Each API implements a distinct protocol on top of FHIR R4 — do not force them into a single generic REST shape.

## General conventions

- Prefer ASP.NET Core minimal APIs, grouped via `MapGroup("/...")`, with controllers as an acceptable alternative for resource-heavy areas — pick one style per project and stay consistent.
- Every endpoint declares `.WithName(...)` and `.WithOpenApi()` (or `[ProducesResponseType]` for controllers) so Swagger accurately reflects the contract.
- Content type for FHIR payloads is `application/fhir+json`; for CDS Hooks payloads it's plain `application/json`.
- Version APIs at the path level only if/when a breaking change is required (e.g. `/v2/...`); do not pre-emptively version before there's a second version.

## Error responses

- Non-FHIR errors (malformed request shape, auth failures): standard ASP.NET Core `ProblemDetails` with an appropriate status code (`400`, `401`, `403`, `404`, `409`).
- FHIR-facing errors (invalid/non-conformant resource): a FHIR `OperationOutcome` resource with `issue.severity`, `issue.code`, and human-readable `issue.diagnostics` — returned with `application/fhir+json` and the matching HTTP status (`400` for invalid input, `422` for semantically invalid-but-well-formed resources, `500` reserved for genuine server faults).
- Never leak stack traces or internal exception messages to clients; log details server-side (without PHI) and return a generic diagnostic message plus a correlation ID.

## Per-API contract summary

### CRD.Api (CDS Hooks)
- `GET /cds-services` — discovery document.
- `POST /cds-services/{id}` — hook invocation; request/response follow the CDS Hooks spec (`Card[]`, `systemActions`), not generic REST semantics.
- Secured via CDS Hooks service-token bearer auth.

### DTR.Api (SMART on FHIR)
- SMART App Launch endpoints (`/launch`, `/callback` or equivalent) per the SMART App Launch framework.
- `GET` endpoint(s) to retrieve the applicable `Questionnaire` (with pre-population applied).
- `POST` endpoint to submit the completed `QuestionnaireResponse`.
- Secured via SMART on FHIR OAuth2 (authorization code + PKCE).

### PAS.Api (FHIR Claim/ClaimResponse)
- `POST /Claim/$submit` — submit a prior authorization request `Bundle`; returns a `Bundle` containing a `ClaimResponse` (synchronous) or a `ClaimResponse` with `outcome = queued` (asynchronous).
- `GET`/`POST /Claim/$inquire` (or a `Task`-based status endpoint) — poll for the outcome of a pended request.
- Secured via backend-services (client-credentials + signed JWT assertion).

## Pagination & search

- Any `GET` endpoint returning a FHIR `searchset` Bundle supports standard FHIR search parameters and pagination via `Bundle.link` (`self`, `next`) — do not invent a non-FHIR pagination scheme for FHIR search endpoints.
- Non-FHIR list endpoints (if any, e.g. internal admin/reporting) use simple `page`/`pageSize` query parameters with total count in the response.

## Swagger / OpenAPI

- Every API project keeps `AddEndpointsApiExplorer()` + `AddSwaggerGen()` (or equivalent) enabled in Development, and documents request/response examples for the endpoints above so the generated spec is usable for the Postman collection (see [generate-postman.prompt.md](../.github/prompts/generate-postman.prompt.md)).
