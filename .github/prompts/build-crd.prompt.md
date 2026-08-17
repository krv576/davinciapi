---
mode: agent
description: Implement the CRD (Coverage Requirements Discovery) CDS Hooks API
---

# Build CRD

Implement `DavinciEPA.CRD.Api`, the CDS Hooks service for Coverage Requirements Discovery.

## Goal

Expose the CDS Hooks discovery and hook-invocation endpoints so an EHR's CDS client can learn, at order time, what documentation/prior-auth a payer requires.

## Prerequisites

`Core`, `Fhir`, `Rules`, and `Security` should already provide: coverage requirement domain services, FHIR resource builders for referenced orders, a rule engine for coverage evaluation, and CDS Hooks service-token validation.

## Steps

1. Implement `GET /cds-services` returning the service discovery document (`services[]` with `hook`, `id`, `title`, `description`, `prefetch`) for the hooks this deployment supports (e.g. `order-select`, `order-sign`).
2. Implement `POST /cds-services/{id}` per hook:
   - Deserialize the CDS Hooks request (`hook`, `hookInstance`, `context`, `prefetch`).
   - Resolve/validate any resources not supplied via `prefetch` using a FHIR client against the calling EHR's FHIR server, when configured.
   - Invoke the `Core` application service to evaluate coverage requirements for the order in context (delegating rule evaluation to `DavinciEPA.Rules`).
   - Map the result to CDS Hooks `Card` objects (`summary`, `indicator`, `detail`, and a `link` of type `smart` pointing to the DTR SMART app when documentation is required).
3. Validate the CDS Hooks service token per [security.instructions.md](../instructions/security.instructions.md).
4. Register all services in `Program.cs` (composition root only — no logic there).
5. Add unit tests in `tests/CRD.Tests` for hook request/response mapping and rule-evaluation edge cases; add an integration test in `tests/Integration.Tests` exercising `POST /cds-services/{id}` end-to-end with `WebApplicationFactory`.

## Acceptance criteria

- `/cds-services` returns a valid discovery document.
- Hook responses are valid CDS Hooks `Card` JSON (not raw FHIR resources).
- No business/rule-evaluation logic lives in the controller/minimal-API handler itself.
