---
applyTo: "src/DavinciEPA.Infrastructure/**"
---

# Database & Persistence Instructions

Applies to `DavinciEPA.Infrastructure`, which owns all data access for the solution.

## Principles

- No other project may reference a database driver, `DbContext`, or ORM package directly — all persistence goes through repository interfaces defined in `DavinciEPA.Core` and implemented here.
- Entities used for persistence (EF Core entities) are separate types from FHIR resources and domain models; map between them explicitly in the repository implementation rather than persisting FHIR POCOs directly.

## EF Core conventions

- Use **EF Core** with a `DbContext` per bounded area if needed, or a single `DavinciEpaDbContext` if the schema stays small — prefer the single context until there's a clear reason to split.
- Configure entities via `IEntityTypeConfiguration<T>` classes (Fluent API) in a `Configurations/` folder — avoid data annotations scattered on entity classes.
- Every schema change ships with an EF Core migration (`dotnet ef migrations add <Name>`), committed alongside the code change. Never edit an already-applied/shared migration; add a new one.
- Use `async` EF Core APIs (`ToListAsync`, `SaveChangesAsync`, etc.) with a `CancellationToken` end-to-end.

## Connection strings & secrets

- Connection strings are never committed in `appsettings.json`; use `appsettings.Development.json` (gitignored per-developer values), user-secrets, or environment variables in non-dev environments (see [security.instructions.md](security.instructions.md)).

## Data model guidance

- Persist operational/workflow state (e.g. prior-auth request status, CRD rule evaluation results, DTR questionnaire progress, audit trail) — do not persist entire FHIR Bundles as opaque blobs when structured querying is needed; extract queryable fields into columns and keep the raw resource (if retained) in a JSON column for traceability.
- Add indexes for fields used in lookups (e.g. `PriorAuthRequest.ExternalId`, `PatientIdentifier`, `Status`).
- See [docs/database-design.md](../../docs/database-design.md) for the current/target entity list.

## Repository pattern

- Define one interface per aggregate in `Core` (e.g. `IPriorAuthRequestRepository`), implement it in `Infrastructure`, and register it in the consuming API's `Program.cs` via DI.
- Repository methods return domain/application models, not `DbSet<T>` or `IQueryable<T>`, to keep query composition inside `Infrastructure`.
