---
mode: agent
description: Implement DavinciEPA.Infrastructure persistence and external HTTP clients
---

# Build Infrastructure

Implement `DavinciEPA.Infrastructure`: EF Core persistence and external HTTP clients (payer/EHR FHIR endpoints) that back the `Core` port interfaces.

## Goal

Provide concrete, swappable implementations of `Core` repository/client interfaces, keeping data-access and outbound-HTTP concerns out of every other layer.

## Steps

1. Add EF Core packages to `DavinciEPA.Infrastructure.csproj` (`Microsoft.EntityFrameworkCore`, plus the provider — e.g. `Npgsql.EntityFrameworkCore.PostgreSQL` or `Microsoft.EntityFrameworkCore.SqlServer`) matching whatever the team has chosen for this environment.
2. Define a `DavinciEpaDbContext` and per-entity `IEntityTypeConfiguration<T>` classes under `Persistence/Configurations/` for the entities in [docs/database-design.md](../../docs/database-design.md).
3. Implement each `Core` repository interface (e.g. `IPriorAuthRequestRepository`) as an EF Core-backed class in `Persistence/Repositories/`, mapping between EF entities and `Core` domain models explicitly.
4. Generate the initial EF Core migration (`dotnet ef migrations add InitialCreate --project src/DavinciEPA.Infrastructure --startup-project src/DavinciEPA.PAS.Api`) once the model is stable enough for a first cut.
5. Implement outbound FHIR/HTTP clients (e.g. `IPayerFhirClient`, `IEhrFhirClient`) using `HttpClientFactory` (`AddHttpClient<T>`), with resilience (timeout/retry) policies and auth headers supplied via `DavinciEPA.Security`.
6. Register everything via an `AddInfrastructure(IConfiguration)` extension method so each API's `Program.cs` stays a thin composition root.
7. Follow [database.instructions.md](../instructions/database.instructions.md) and [security.instructions.md](../instructions/security.instructions.md) (no secrets/connection strings committed).
8. Add tests: repository tests against an in-memory or test-container database, and HTTP client tests using a fake `HttpMessageHandler`.

## Acceptance criteria

- `DavinciEPA.Infrastructure` is the only project referencing EF Core or a raw `HttpClient` for external payer/EHR calls.
- All repository/client interfaces are defined in `Core` and merely implemented here.
- A working EF Core migration exists and applies cleanly to a fresh database.
