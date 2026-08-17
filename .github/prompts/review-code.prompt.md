---
mode: agent
description: Review a DavinciEPA code change for architecture, FHIR, security, and test compliance
---

# Review Code

Review the current change set (staged/unstaged diff, or the files under discussion) against this repository's standards before it is considered done.

## Checklist

### Architecture
- [ ] No project reference violates the dependency graph in [copilot-instructions.md](../copilot-instructions.md) / [AGENTS.md](../AGENTS.md).
- [ ] No business logic added to `Program.cs` or a controller/minimal-API handler body beyond request mapping and calling a `Core` service.
- [ ] New port interfaces live in `Core`; implementations live in the correct outer layer (`Fhir`, `Infrastructure`, `Rules`, `Security`).

### FHIR (if applicable)
- [ ] FHIR resources are built/parsed only via `DavinciEPA.Fhir`, using Firely SDK types.
- [ ] `Meta.Profile` and all mandatory elements are set for any new/changed resource.
- [ ] Validation failures surface as `OperationOutcome`, not unhandled exceptions.

### Security
- [ ] No secrets, connection strings, or keys committed.
- [ ] Auth/token validation is not weakened, bypassed, or duplicated outside `DavinciEPA.Security`.
- [ ] No PHI appears in logs, exception messages, or test fixtures.
- [ ] External input is validated; no string-concatenated queries.

### Persistence (if applicable)
- [ ] Schema changes include an EF Core migration.
- [ ] Repository interfaces stay in `Core`; EF Core types don't leak outside `Infrastructure`.

### Tests
- [ ] New/changed logic has unit tests in the matching `tests/*.Tests` project.
- [ ] Cross-project/endpoint changes have an `Integration.Tests` case.
- [ ] `dotnet build DavinciEPA.sln` and `dotnet test DavinciEPA.sln` pass.

### Style
- [ ] Matches [coding-standards.md](../../docs/coding-standards.md) (nullable-safe, async/await with `CancellationToken`, naming conventions).
- [ ] No dead code, commented-out blocks, or unrelated formatting churn.

## Output

Summarize findings as a short list of ✅/⚠️/❌ items with file references, then propose concrete fixes for anything ⚠️/❌ rather than only flagging it.
