---
applyTo: "src/**/*.cs"
---

# Backend (ASP.NET Core / C#) Instructions

Applies to all C# source under `src/`.

## Project conventions

- Target `net8.0`, `Nullable` enabled, `ImplicitUsings` enabled — do not disable these in new projects.
- One class/interface/record per file; file name matches the type name.
- Namespaces follow folder structure, rooted at the project name (e.g. `DavinciEPA.Core.PriorAuthorization`).
- Use `record` for immutable DTOs/value objects, `class` for services and entities with identity.

## API projects (`*.Api`)

- Prefer minimal APIs (`app.MapGet/MapPost` grouped via `MapGroup`) for simple endpoints, or controllers when a resource has many related operations — be consistent within a single API project.
- `Program.cs` is a composition root only: service registration, middleware pipeline, endpoint mapping. No business logic.
- Register application services from `Core` behind interfaces; register their `Infrastructure`/`Fhir`/`Security` implementations via `AddScoped`/`AddSingleton` in `Program.cs`.
- All endpoints must specify an explicit route, `.WithName(...)`, and `.WithOpenApi()`/`[ProducesResponseType]` so Swagger stays accurate.
- Validate input DTOs before invoking domain logic; return `400` with a `ProblemDetails` (or FHIR `OperationOutcome`, see [fhir.instructions.md](fhir.instructions.md)) on failure — never let unhandled exceptions bubble to the client.

## Core/domain services

- Application services expose async methods (`Task`/`Task<T>`) and accept a `CancellationToken` as the last parameter.
- Depend on interfaces (ports) defined in `Core`, injected via constructor — no static service locators.
- Domain/business errors are modeled explicitly (e.g. a `Result<T>`/error type in `Shared`), not thrown as exceptions for expected failure paths. Reserve exceptions for truly exceptional conditions.

## Error handling & logging

- Use the built-in `ILogger<T>` via DI; do not use `Console.WriteLine`.
- Log at `Information` for request-level events, `Warning` for recoverable issues, `Error` for failures. Never log PHI (patient identifiers, clinical notes) — log resource types/IDs and correlation IDs instead.
- Use a global exception handler/middleware in each `*.Api` project to translate unhandled exceptions into a consistent error response.

## Configuration

- Read configuration via strongly-typed `IOptions<T>` bound from `appsettings.json`, not `IConfiguration["Key"]` scattered through the codebase.
- Environment-specific values go in `appsettings.{Environment}.json`; secrets never go in any `appsettings*.json` file (see [security.instructions.md](security.instructions.md)).
