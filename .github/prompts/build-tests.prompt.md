---
mode: agent
description: Scaffold or extend unit and integration tests for a DavinciEPA feature
---

# Build Tests

Add or extend automated tests for a DavinciEPA feature, following [testing.instructions.md](../instructions/testing.instructions.md).

## Goal

Ensure new/changed behavior in any layer is covered by unit tests in the matching `tests/*.Tests` project, and by an integration test in `tests/Integration.Tests` when the change crosses project boundaries (e.g. a new endpoint).

## Steps

1. Identify which `tests/*` project matches the code under test:
   - `CRD.Tests` — `DavinciEPA.CRD.Api` and CRD-specific logic.
   - `DTR.Tests` — `DavinciEPA.DTR.Api` and DTR-specific logic.
   - `PAS.Tests` — `DavinciEPA.PAS.Api` and PAS-specific logic.
   - `Integration.Tests` — cross-project, end-to-end scenarios via `WebApplicationFactory<Program>`.
2. Mirror the folder/namespace of the code under test inside the test project.
3. Write unit tests using xUnit + a single consistent mocking library for the project (`Moq` unless the project has already standardized on something else); cover the happy path plus at least one failure/edge case per new method.
4. For FHIR-facing code, assert on resource shape (required elements populated, `Meta.Profile` set) rather than just "it didn't throw".
5. For new/changed endpoints, add or update an integration test asserting HTTP status code and response shape end-to-end.
6. Use synthetic, obviously-fake data only — never real-looking PHI (see [testing.instructions.md](../instructions/testing.instructions.md)).
7. Run the affected test project(s) and the full suite before finishing:

```bash
dotnet test tests/<Project>.Tests
dotnet test DavinciEPA.sln
```

## Acceptance criteria

- All new/changed logic has at least one passing unit test and, where applicable, an integration test.
- `dotnet test DavinciEPA.sln` passes with no failing or skipped-without-reason tests.
