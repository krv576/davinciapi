---
mode: agent
description: Scaffold DavinciEPA.Fhir resource builders, parsers, and profile validation
---

# Build Fhir

Implement `DavinciEPA.Fhir`, the layer responsible for all FHIR R4 resource construction, parsing, and Da Vinci profile conformance.

## Goal

Provide FHIR resource builders/mappers consumed by `Core` (via interfaces) and the `*.Api` projects, using the Firely .NET SDK.

## Steps

1. Add the Firely SDK NuGet packages to `DavinciEPA.Fhir.csproj` (`Hl7.Fhir.R4`), consistent with [fhir.instructions.md](../instructions/fhir.instructions.md).
2. For each Da Vinci resource needed (see [docs/fhir-resources.md](../../docs/fhir-resources.md)), implement a builder/mapper class in `DavinciEPA.Fhir/{CRD|DTR|PAS}/` that:
   - Implements the corresponding `Core` port interface (e.g. `IFhirResourceBuilder<PriorAuthRequest, Claim>`).
   - Sets `Meta.Profile` to the canonical Da Vinci profile URL.
   - Populates all mandatory elements for that profile.
3. Implement parsing (FHIR JSON → domain model) counterparts for inbound resources (e.g. incoming `Bundle` for PAS `$submit`, incoming `QuestionnaireResponse` for DTR).
4. Add a `IResourceValidator`-based validation helper that checks a constructed/parsed resource against its profile and returns an `OperationOutcome` on failure.
5. Configure JSON (de)serialization once (e.g. a shared `FhirJsonSerializerSettings`/parser settings instance) rather than re-instantiating settings per call site.
6. Add unit tests in the relevant `tests/*.Tests` project asserting: required elements are populated, `Meta.Profile` is set, and round-trip parse/serialize is stable.

## Acceptance criteria

- `DavinciEPA.Fhir` compiles referencing only `Core` and `Shared` as project references (plus the Firely NuGet packages).
- No FHIR resource is constructed anywhere outside `DavinciEPA.Fhir`.
- Validation failures produce a FHIR `OperationOutcome`, not an unhandled exception.
