---
mode: agent
description: Implement DavinciEPA.Security authentication/authorization for CRD, DTR, and PAS
---

# Build Security

Implement `DavinciEPA.Security`, covering the distinct auth models required by CRD, DTR, and PAS.

## Goal

Provide reusable authentication/authorization building blocks so each API project only needs to reference and configure `DavinciEPA.Security`, never reimplement token handling.

## Steps

1. **CDS Hooks (CRD)**: implement service-token validation for inbound CDS Hooks calls per the CDS Hooks security spec (JWT bearer, issuer = calling EHR, audience = this service).
2. **SMART App Launch (DTR)**: implement the authorization code + PKCE flow helper (building the authorize redirect, handling the callback, exchanging the code for a token) and launch-context validation (`patient`, `encounter`, `need_patient_banner`, scopes).
3. **Backend services / client-credentials (PAS)**: implement JWT client-assertion creation (signed with this system's private key) for outbound token requests, and inbound bearer-token validation for `$submit`/`$inquire` callers.
4. Expose configuration via strongly-typed options bound from `appsettings.json` (issuer URLs, audiences, key references) — never hard-code endpoints or keys.
5. Provide `IServiceCollection` extension methods (e.g. `AddCrdSecurity()`, `AddDtrSecurity()`, `AddPasSecurity()`) so each API's `Program.cs` stays a thin composition root.
6. Follow [security.instructions.md](../instructions/security.instructions.md) strictly: no disabled validation, no secrets in source, PHI-safe logging.
7. Add unit tests for token/claims validation logic (valid, expired, wrong-audience, wrong-issuer, missing-scope cases).

## Acceptance criteria

- Each API project configures auth via a single `AddXxxSecurity()` call from `DavinciEPA.Security`.
- All three auth models (CDS Hooks service token, SMART launch, backend-services client-credentials) are implemented and independently testable.
- No token validation logic is duplicated inside an `*.Api` project.
