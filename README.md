# DavinciEPA

A production-grade implementation of the HL7 **Da Vinci Electronic Prior Authorization (EPA)** workflow on ASP.NET Core / .NET 8, covering:

- **CRD** — Coverage Requirements Discovery (CDS Hooks)
- **DTR** — Documentation Templates and Rules (SMART on FHIR)
- **PAS** — Prior Authorization Support (FHIR `Claim`/`ClaimResponse`, replacing X12 278)

## Tech stack

- .NET 8, ASP.NET Core (minimal APIs)
- HL7 FHIR R4 via the Firely .NET SDK
- Entity Framework Core / SQL Server
- OAuth2 / OpenID Connect / JWT / SMART on FHIR
- xUnit + FluentAssertions
- Serilog
- Swagger / OpenAPI

## Solution structure

```
src/
  DavinciEPA.Shared/          Cross-cutting primitives
  DavinciEPA.Core/            Domain models, application services, interfaces
  DavinciEPA.Fhir/             FHIR R4 resource builders/parsers/validation
  DavinciEPA.Rules/            Coverage & documentation rule evaluation
  DavinciEPA.Security/         AuthN/AuthZ (OAuth2, SMART, CDS Hooks tokens)
  DavinciEPA.Infrastructure/    EF Core persistence, external HTTP clients
  DavinciEPA.CRD.Api/          Coverage Requirements Discovery API
  DavinciEPA.DTR.Api/          Documentation Templates and Rules API
  DavinciEPA.PAS.Api/          Prior Authorization Support API
tests/
  CRD.Tests/ DTR.Tests/ PAS.Tests/  Unit tests
  Integration.Tests/                End-to-end tests
```

Full details: [docs/architecture.md](docs/architecture.md) and [docs/folder-structure.md](docs/folder-structure.md).

## Getting started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local or containerized) for `DavinciEPA.Infrastructure`

### Build

```bash
dotnet restore DavinciEPA.sln
dotnet build DavinciEPA.sln
```

### Run an API

```bash
dotnet run --project src/DavinciEPA.CRD.Api
dotnet run --project src/DavinciEPA.DTR.Api
dotnet run --project src/DavinciEPA.PAS.Api
```

Each API exposes Swagger UI at `/swagger` in the Development environment.

### Test

```bash
dotnet test DavinciEPA.sln
```

## Documentation

| Doc | Purpose |
|---|---|
| [docs/architecture.md](docs/architecture.md) | Layered architecture, dependency rules, end-to-end workflow |
| [docs/folder-structure.md](docs/folder-structure.md) | Full repository layout and where new code belongs |
| [docs/coding-standards.md](docs/coding-standards.md) | C# conventions used across the solution |
| [docs/api-design.md](docs/api-design.md) | HTTP/FHIR API conventions per project |
| [docs/fhir-resources.md](docs/fhir-resources.md) | FHIR resources and Da Vinci profiles used per IG |
| [docs/database-design.md](docs/database-design.md) | Persistence entity model |
| [docs/development-roadmap.md](docs/development-roadmap.md) | Phased build plan |
| [docs/implementation-order.md](docs/implementation-order.md) | Concrete bottom-up build sequence |
| [docs/testing-strategy.md](docs/testing-strategy.md) | Unit/integration/conformance testing approach |

## Working with GitHub Copilot

This repository ships repository-wide and path-scoped Copilot instructions, plus reusable prompts:

- [.github/copilot-instructions.md](.github/copilot-instructions.md) — repository-wide guidance
- [.github/AGENTS.md](.github/AGENTS.md) — AI agent behavior and architecture rules
- [.github/instructions/](.github/instructions/) — path-scoped rules (backend, FHIR, testing, database, security)
- [.github/prompts/](.github/prompts/) — task prompts (e.g. `build-core`, `build-fhir`, `build-crd`, `review-code`)

## Security

Never disable authentication, log PHI, or commit secrets/connection strings. See [.github/instructions/security.instructions.md](.github/instructions/security.instructions.md).
