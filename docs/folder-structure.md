# Folder Structure

```
DavinciEPA.sln
.github/
  copilot-instructions.md        Repository-wide Copilot instructions
  AGENTS.md                      AI agent behavior and architecture rules
  instructions/                  Path-scoped Copilot instructions
    backend.instructions.md
    fhir.instructions.md
    testing.instructions.md
    database.instructions.md
    security.instructions.md
  prompts/                       Reusable Copilot Chat prompts
    build-core.prompt.md
    build-fhir.prompt.md
    build-crd.prompt.md
    build-dtr.prompt.md
    build-pas.prompt.md
    build-rules.prompt.md
    build-security.prompt.md
    build-infrastructure.prompt.md
    build-tests.prompt.md
    review-code.prompt.md
    generate-postman.prompt.md
docs/                            Architecture & process documentation (this folder)
src/
  DavinciEPA.Shared/              Cross-cutting primitives, no project dependencies
  DavinciEPA.Core/                Domain models, application services, port interfaces
  DavinciEPA.Fhir/                FHIR R4 resource builders/parsers/validation (Firely SDK)
  DavinciEPA.Rules/               Coverage/documentation rule evaluation engine
  DavinciEPA.Security/            AuthN/AuthZ: CDS Hooks tokens, SMART launch, client-credentials
  DavinciEPA.Infrastructure/       EF Core persistence, outbound payer/EHR HTTP clients
  DavinciEPA.CRD.Api/              CDS Hooks API (Coverage Requirements Discovery)
  DavinciEPA.DTR.Api/              SMART on FHIR API (Documentation Templates and Rules)
  DavinciEPA.PAS.Api/              FHIR Claim/ClaimResponse API (Prior Authorization Support)
tests/
  CRD.Tests/                      Unit tests for DavinciEPA.CRD.Api
  DTR.Tests/                      Unit tests for DavinciEPA.DTR.Api
  PAS.Tests/                      Unit tests for DavinciEPA.PAS.Api
  Integration.Tests/               End-to-end tests across the full stack
README.md
```

## Project dependency rules

See the dependency graph in [architecture.md](architecture.md). Summary:

- `Shared` has no project references.
- `Core` references only `Shared`.
- `Fhir`, `Rules`, `Security` reference `Core`/`Shared` (and `Rules` also references `Fhir`).
- `Infrastructure` references `Core`, `Fhir`, `Security`, `Shared`.
- `*.Api` projects reference all of the above as needed; they are never referenced by any other project.
- `tests/*` projects reference only the project(s) they test (`Integration.Tests` may reference multiple `*.Api` projects).

## Where new code goes

| Adding... | Goes in |
|---|---|
| A new domain concept or use case | `DavinciEPA.Core` |
| A new FHIR resource builder/parser | `DavinciEPA.Fhir` |
| A new coverage/documentation rule | `DavinciEPA.Rules` |
| A new auth flow or token validation | `DavinciEPA.Security` |
| A new EF Core entity/repository or outbound HTTP client | `DavinciEPA.Infrastructure` |
| A new CRD/DTR/PAS HTTP endpoint | the matching `*.Api` project |
| Tests for any of the above | the matching project under `tests/` |
