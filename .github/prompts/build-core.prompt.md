---
mode: agent
description: Scaffold DavinciEPA.Core domain models, use cases, and port interfaces
---

# Build Core

Implement `DavinciEPA.Core`, the domain/application layer shared by CRD, DTR, and PAS.

## Goal

Produce the domain models and application services that the FHIR, Rules, Infrastructure, and API layers will depend on — with **no** dependency on FHIR types, EF Core, or ASP.NET Core.

## Steps

1. Reference [docs/architecture.md](../../docs/architecture.md) and [docs/database-design.md](../../docs/database-design.md) to identify the core aggregates: e.g. `PriorAuthRequest`, `CoverageRequirement`, `DocumentationRequirement`, `RuleEvaluationResult`.
2. Define domain models as plain C# records/classes in `DavinciEPA.Core/Domain/` (or per-aggregate subfolders). Use `DavinciEPA.Shared` types (`Result<T>`, error types) for outcomes instead of throwing for expected failures.
3. Define **port interfaces** for anything an outer layer must implement, under `DavinciEPA.Core/Interfaces/` (or colocated with the aggregate), for example:
   - `IPriorAuthRequestRepository`
   - `ICoverageRequirementService`
   - `IFhirResourceBuilder<T>` (implemented in `Fhir`)
   - `IRuleEngine` (implemented in `Rules`)
4. Implement application services/use cases (e.g. `SubmitPriorAuthorizationService`, `EvaluateCoverageRequirementsService`) that orchestrate domain models + port interfaces. Accept a `CancellationToken`, return `Task`/`Task<Result<T>>`.
5. Follow [backend.instructions.md](../instructions/backend.instructions.md) for naming/error-handling conventions.
6. Add unit tests for each new service under the matching `tests/*.Tests` project (whichever API consumes it first), per [testing.instructions.md](../instructions/testing.instructions.md).
7. Run `dotnet build src/DavinciEPA.Core` and confirm the project still has zero references beyond `DavinciEPA.Shared`.

## Acceptance criteria

- `DavinciEPA.Core.csproj` only references `DavinciEPA.Shared`.
- No `Hl7.Fhir.*`, `Microsoft.EntityFrameworkCore.*`, or `Microsoft.AspNetCore.*` usings appear anywhere in `DavinciEPA.Core`.
- New services are covered by unit tests.
