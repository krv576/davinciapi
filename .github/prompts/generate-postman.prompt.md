---
mode: agent
description: Generate a Postman collection covering the CRD, DTR, and PAS API endpoints
---

# Generate Postman Collection

Produce a Postman collection (and matching environment file) covering the currently implemented endpoints across `DavinciEPA.CRD.Api`, `DavinciEPA.DTR.Api`, and `DavinciEPA.PAS.Api`.

## Steps

1. Inspect each API project's endpoints/controllers (and the generated OpenAPI/Swagger document at `/swagger/v1/swagger.json` when the API is run locally) to enumerate current routes, methods, and request/response shapes.
2. Create a Postman collection JSON (schema `v2.1.0`) with one folder per API (`CRD`, `DTR`, `PAS`), and one request per endpoint, named after the operation (e.g. `CRD > GET cds-services`, `PAS > POST Claim/$submit`).
3. Use collection/environment variables for host + port per API (e.g. `{{crdBaseUrl}}`, `{{dtrBaseUrl}}`, `{{pasBaseUrl}}`) instead of hard-coded `localhost` URLs, plus a `{{accessToken}}` variable wired into an `Authorization: Bearer {{accessToken}}` header on secured requests.
4. Include representative example request bodies built from realistic-but-synthetic FHIR resources consistent with [fhir.instructions.md](../instructions/fhir.instructions.md) (correct `resourceType`, required elements, `Meta.Profile`) — never real patient data.
5. Add a Postman environment file with placeholder values for the base URLs and an empty `accessToken` variable to be filled in per developer.
6. Save the collection as `postman/DavinciEPA.postman_collection.json` and the environment as `postman/DavinciEPA.postman_environment.json` in the repository root, creating the `postman/` folder if it doesn't exist.

## Acceptance criteria

- Collection imports cleanly into Postman with no manual fixes required.
- Every currently implemented endpoint across the three APIs has a corresponding request.
- No hard-coded hostnames, ports, tokens, or real-looking patient identifiers appear anywhere in the generated files.
