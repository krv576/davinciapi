---
mode: agent
description: Implement the DTR (Documentation Templates and Rules) SMART on FHIR API
---

# Build DTR

Implement `DavinciEPA.DTR.Api`, the backend supporting the SMART on FHIR Documentation Templates and Rules app.

## Goal

Serve the `Questionnaire` the DTR app renders, accept the completed `QuestionnaireResponse`, evaluate any embedded CQL/FHIRPath rules that auto-populate answers from the EHR, and package the result for PAS submission.

## Prerequisites

`Core` (documentation-requirement services), `Fhir` (Questionnaire/QuestionnaireResponse builders/parsers), `Rules` (CQL/FHIRPath evaluation), and `Security` (SMART App Launch) should exist first.

## Steps

1. Implement SMART App Launch support: accept the `launch`/`iss` parameters, complete the authorization code + PKCE exchange via `DavinciEPA.Security`, and obtain a FHIR access token scoped to the launching patient/encounter context.
2. Implement `GET` endpoint(s) to retrieve the `Questionnaire` applicable to the triggering order (keyed by the order/coverage requirement identified during CRD), via the `Core` documentation-requirement service.
3. Implement rule-based pre-population: evaluate embedded rules (CQL/FHIRPath expressions per the DTR IG's `QuestionnaireResponse` population extensions) against the EHR's FHIR data to pre-fill answers, using `DavinciEPA.Rules`.
4. Implement `POST` endpoint to accept the completed `QuestionnaireResponse`, validate it against the DTR profile via `DavinciEPA.Fhir`, and persist it via `DavinciEPA.Infrastructure`.
5. Provide a handoff step that packages the `QuestionnaireResponse` (and referenced supporting resources) into the `Bundle` PAS expects for `$submit`.
6. Add unit tests in `tests/DTR.Tests` for rule-based pre-population and validation; add an integration test covering launch → fetch questionnaire → submit response.

## Acceptance criteria

- Launch context is validated before any patient data is returned.
- Returned/accepted resources conform to the DTR `Questionnaire`/`QuestionnaireResponse` profiles.
- No CQL/FHIRPath evaluation logic is duplicated in the API project — it lives in `DavinciEPA.Rules`.
