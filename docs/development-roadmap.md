# Development Roadmap

Phased plan for building out DavinciEPA from the current scaffold to a production-grade EPA platform. Each phase should be completed (build green, tests passing) before starting the next, per the dependency graph in [architecture.md](architecture.md).

## Phase 0 — Scaffold (done)
- Solution/project structure created for all `src/` and `tests/` projects.
- Copilot instructions, prompts, and documentation established (this folder).

## Phase 1 — Shared & Core foundations
- `DavinciEPA.Shared`: `Result<T>`, domain error types, common constants/extensions.
- `DavinciEPA.Core`: domain models for prior-auth workflow (`PriorAuthRequest`, `CoverageRequirement`, `DocumentationRequirement`), port interfaces for repositories/FHIR builders/rule engine/external clients.
- Prompt: [build-core.prompt.md](../.github/prompts/build-core.prompt.md).

## Phase 2 — FHIR layer
- `DavinciEPA.Fhir`: Firely SDK integration, resource builders/parsers for CRD/DTR/PAS profiles, profile validation.
- Prompt: [build-fhir.prompt.md](../.github/prompts/build-fhir.prompt.md).

## Phase 3 — Security layer
- `DavinciEPA.Security`: CDS Hooks service-token validation, SMART App Launch, backend-services client-credentials.
- Prompt: [build-security.prompt.md](../.github/prompts/build-security.prompt.md).

## Phase 4 — Infrastructure layer
- `DavinciEPA.Infrastructure`: EF Core `DbContext`, entity configurations, repository implementations, initial migration, outbound payer/EHR FHIR HTTP clients.
- Prompt: [build-infrastructure.prompt.md](../.github/prompts/build-infrastructure.prompt.md).

## Phase 5 — Rules engine
- `DavinciEPA.Rules`: coverage requirement evaluation (CRD) and documentation pre-population (DTR).
- Prompt: [build-rules.prompt.md](../.github/prompts/build-rules.prompt.md).

## Phase 6 — CRD API
- CDS Hooks discovery + hook invocation endpoints, coverage requirement Cards, SMART link to DTR.
- Prompt: [build-crd.prompt.md](../.github/prompts/build-crd.prompt.md).

## Phase 7 — DTR API
- SMART App Launch, Questionnaire retrieval + pre-population, QuestionnaireResponse submission, PAS handoff packaging.
- Prompt: [build-dtr.prompt.md](../.github/prompts/build-dtr.prompt.md).

## Phase 8 — PAS API
- `$submit`/`$inquire` operations, synchronous and asynchronous adjudication flows, ClaimResponse generation.
- Prompt: [build-pas.prompt.md](../.github/prompts/build-pas.prompt.md).

## Phase 9 — Cross-cutting hardening
- Full unit + integration test coverage (prompt: [build-tests.prompt.md](../.github/prompts/build-tests.prompt.md)).
- Security review pass (prompt: [review-code.prompt.md](../.github/prompts/review-code.prompt.md)) against [security.instructions.md](../.github/instructions/security.instructions.md).
- Structured logging/observability, health checks, and Swagger polish across all three APIs.

## Phase 10 — Integration & tooling
- `Integration.Tests` covering the full CRD → DTR → PAS happy path and key failure paths.
- Postman collection for manual verification (prompt: [generate-postman.prompt.md](../.github/prompts/generate-postman.prompt.md)).
- FHIR conformance verification against Da Vinci profiles (e.g. via a Touchstone/Inferno-style test suite) before considering an IG "supported".

See [implementation-order.md](implementation-order.md) for the concrete build sequence within each phase.
