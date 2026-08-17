---
mode: agent
description: Implement the PAS (Prior Authorization Support) Claim/ClaimResponse API
---

# Build PAS

Implement `DavinciEPA.PAS.Api`, the FHIR-based prior authorization exchange that replaces the X12 278 transaction.

## Goal

Accept a prior authorization request (`Claim` with `use = preauthorization`, wrapped in a `Bundle`) via the `$submit` operation, adjudicate/route it, and return a `ClaimResponse` — synchronously or asynchronously per the PAS IG.

## Prerequisites

`Core` (prior-auth request lifecycle/state machine), `Fhir` (Claim/ClaimResponse/Bundle builders and parsers), `Infrastructure` (persistence of requests/status), and `Security` (backend-services client-credentials auth) should exist first.

## Steps

1. Implement `POST /Claim/$submit`: accept the request `Bundle`, validate it against the PAS `Bundle`/`Claim` profile via `DavinciEPA.Fhir`, and hand off to the `Core` prior-auth service.
2. Implement the adjudication/routing flow in `Core` (approve/deny/pend), persisting request + decision state via `DavinciEPA.Infrastructure`.
3. For synchronous decisions, return a `Bundle` containing the `ClaimResponse` immediately. For pended decisions, return a `ClaimResponse` with `outcome = queued` and implement `GET /Claim/$inquire` (or a `Task`-based status endpoint) for polling, per the PAS IG's asynchronous pattern.
4. Validate the caller via backend-services client-credentials auth per [security.instructions.md](../instructions/security.instructions.md).
5. Ensure all persisted request/decision records support audit/history queries (see [docs/database-design.md](../../docs/database-design.md)).
6. Add unit tests in `tests/PAS.Tests` for the adjudication/state-machine logic and Bundle validation; add an integration test covering submit → (optional) inquire → final `ClaimResponse`.

## Acceptance criteria

- `$submit` validates the inbound `Bundle`/`Claim` and rejects non-conformant submissions with an `OperationOutcome`.
- Asynchronous ("pended") flows are supported via `$inquire`/status polling, not just synchronous responses.
- All state transitions are persisted and auditable.
