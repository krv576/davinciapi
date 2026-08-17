---
mode: agent
description: Implement the DavinciEPA.Rules coverage/documentation rule evaluation engine
---

# Build Rules

Implement `DavinciEPA.Rules`, the engine that evaluates coverage requirements (CRD) and documentation/pre-population rules (DTR) against patient/encounter FHIR data.

## Goal

Provide an `IRuleEngine` (or narrower, per-IG interfaces) implementation that CRD and DTR call into, without either API project embedding rule logic directly.

## Steps

1. Define/confirm the port interface(s) in `DavinciEPA.Core` that this project implements (e.g. `ICoverageRuleEvaluator`, `IQuestionnairePopulationEvaluator`).
2. Choose and implement the rule representation: CQL execution (if a CQL engine dependency is introduced), FHIRPath expression evaluation (via the Firely SDK's FHIRPath support already available through `DavinciEPA.Fhir`), or a simpler declarative rule format for early milestones — document the choice in [docs/architecture.md](../../docs/architecture.md) if it changes from what's already recorded there.
3. Implement rule evaluation against prefetched/fetched FHIR resources (`Condition`, `MedicationRequest`, `Coverage`, etc.), returning a structured result (met/unmet requirements, or pre-population answer values) rather than raw booleans with no context.
4. Keep rule *definitions* (the actual coverage criteria content) data-driven (e.g. JSON/CQL library files loaded at startup or from `Infrastructure`) rather than hard-coded `if` chains scattered through the engine, so payer rules can evolve without redeploys where possible.
5. Add unit tests covering representative rule sets: a requirement that is met, one that is unmet, and one with missing/ambiguous data.

## Acceptance criteria

- `DavinciEPA.Rules` depends only on `Core`, `Fhir`, `Shared`.
- CRD and DTR API projects call into `Rules` only through the `Core`-defined interfaces.
- Rule evaluation results include enough context (which criteria matched/failed) to build a meaningful CDS Hooks `Card` or DTR pre-population answer.
