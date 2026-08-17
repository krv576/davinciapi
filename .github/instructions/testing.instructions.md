---
applyTo: "tests/**/*.cs"
---

# Testing Instructions

Applies to all projects under `tests/`.

## Frameworks

- Test framework: **xUnit**. Assertions via `Assert.*` (or `FluentAssertions` if added to a test project — keep it consistent within that project once introduced).
- Mocking: `Moq` (or `NSubstitute` — pick one per solution and stay consistent; do not mix libraries across projects).
- Test naming: `MethodOrScenario_Condition_ExpectedResult` (e.g. `SubmitPriorAuth_MissingSubscriberId_ReturnsValidationError`).

## Structure

- `CRD.Tests`, `DTR.Tests`, `PAS.Tests` contain **unit tests** for their matching API project and any layer logic exercised only by that workflow. Mirror the namespace/folder of the code under test (e.g. tests for `DavinciEPA.Rules.CoverageEvaluator` live under a `Rules` folder).
- `Integration.Tests` contains **end-to-end tests** that exercise a full API project in-process via `WebApplicationFactory<Program>`, including FHIR request/response round-trips. Use `WebApplicationFactory` with an in-memory/test configuration — never point integration tests at real external payer/EHR endpoints.
- Follow **Arrange / Act / Assert**, with each section either whitespace-separated or comment-labeled for non-trivial tests.

## Test data

- Build FHIR test fixtures using the Firely SDK POCOs (or `.json` sample files under a `TestData`/`Fixtures` folder loaded via `FhirJsonParser`) — never copy real patient data. Use clearly synthetic identifiers (e.g. `Patient/example`, MRN `000000001`).
- Prefer small builder helpers/object mothers for frequently-constructed resources (e.g. a minimal valid PAS `Claim`) over duplicating full JSON payloads in every test.

## Coverage expectations

- New application/domain logic in `Core`, `Rules`, and `Fhir` requires unit tests covering the happy path and at least one failure/edge case.
- New endpoints require an integration test asserting status code and (for FHIR endpoints) that the response is a well-formed resource of the expected type/profile.
- Bug fixes should include a regression test that fails before the fix and passes after.

## Running tests

```bash
dotnet test DavinciEPA.sln                 # everything
dotnet test tests/CRD.Tests                # single project
dotnet test --filter "FullyQualifiedName~SubmitPriorAuth"
```
