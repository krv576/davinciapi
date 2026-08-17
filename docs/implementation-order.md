# Implementation Order

Concrete, dependency-respecting build sequence for the solution. Follow this order so every project compiles against real (not stubbed) dependencies as it's built. Corresponds to the phases in [development-roadmap.md](development-roadmap.md).

1. **`DavinciEPA.Shared`** — `Result<T>`/error types, constants, extensions. No dependencies to worry about; get this right first since everything else builds on it.
2. **`DavinciEPA.Core`** — domain models + port interfaces + application services, using only `Shared`. Define the interfaces (`IPriorAuthRequestRepository`, `IFhirResourceBuilder<>`, `IRuleEngine`, external client ports) that every outer layer will implement.
3. **`DavinciEPA.Fhir`** — implement the FHIR-facing port interfaces from `Core` using the Firely SDK. This unblocks realistic request/response shapes for everything downstream.
4. **`DavinciEPA.Security`** — implement CDS Hooks token validation, SMART App Launch, and backend-services client-credentials, independent of persistence.
5. **`DavinciEPA.Rules`** — implement coverage/documentation rule evaluation using `Core` + `Fhir`. Can be developed in parallel with step 4 since neither depends on the other.
6. **`DavinciEPA.Infrastructure`** — implement repositories (EF Core) and outbound HTTP clients using `Core`, `Fhir`, `Security`. This is the last "library" layer before the APIs.
7. **`DavinciEPA.CRD.Api`** — wire up `Core`, `Fhir`, `Rules`, `Security` behind CDS Hooks endpoints. This is the first runnable API and validates the whole stack end-to-end for the discovery step.
8. **`DavinciEPA.DTR.Api`** — wire up `Core`, `Fhir`, `Rules`, `Security`, `Infrastructure` behind SMART on FHIR endpoints.
9. **`DavinciEPA.PAS.Api`** — wire up `Core`, `Fhir`, `Security`, `Infrastructure` behind the `$submit`/`$inquire` operations, completing the chain.
10. **`tests/*`** — unit tests should be added alongside each step above (not deferred to the end); `Integration.Tests` is completed last, once all three APIs exist, to exercise the full CRD → DTR → PAS flow.

## Practical notes

- Each step should leave `dotnet build DavinciEPA.sln` green — do not move to the next step with a broken build.
- Use the matching prompt in [.github/prompts/](../.github/prompts/) when starting each step (e.g. `build-core.prompt.md` for step 2).
- It's acceptable to revisit an earlier layer (e.g. add a new port interface to `Core`) once a later layer's real requirements are known — update this document's assumptions if the interface surface changes materially.
- Do not build `*.Api` endpoints against interfaces that don't exist yet — implement or stub the real `Core` interface first, per step order.
