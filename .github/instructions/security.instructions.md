---
applyTo: "src/DavinciEPA.Security/**,src/DavinciEPA.*.Api/**"
---

# Security Instructions

Applies to `DavinciEPA.Security` and every API project. This system exchanges healthcare data (potentially PHI) — treat security as a hard requirement, not an afterthought.

## AuthN / AuthZ

- APIs are OAuth2/OIDC resource servers. Validate bearer JWTs using `Microsoft.AspNetCore.Authentication.JwtBearer`, configured with the issuer's real metadata endpoint — never disable signature/issuer/audience validation, including in Development.
- **DTR** additionally implements the **SMART App Launch** framework (authorization code + PKCE) for the embedded questionnaire app; **CRD** is invoked server-to-server via CDS Hooks and should validate the calling EHR's service token per the CDS Hooks security spec; **PAS** uses backend-services (client-credentials with signed JWT assertion) per the Da Vinci PAS IG.
- Enforce scope/claim checks (`[Authorize(Policy = ...)]`) per endpoint — do not rely on `[Authorize]` with no policy for anything touching patient data.
- Keep authentication/authorization logic in `DavinciEPA.Security`; APIs only reference the configured middleware/policies, they don't reimplement token validation.

## Secrets management

- No secrets, client secrets, private keys, or connection strings in source control, `appsettings.json`, or code comments. Use user-secrets locally (`dotnet user-secrets`) and environment variables/a secrets manager (e.g. Azure Key Vault) in deployed environments.
- Signing keys/certificates for outbound JWT assertions are loaded from configuration references (paths/vault URIs), never embedded as literals.

## Data protection (PHI)

- Log resource IDs/correlation IDs, never patient names, DOBs, member IDs, or clinical text.
- Enforce HTTPS everywhere (`UseHttpsRedirection`, HSTS in non-Development) and set secure/`HttpOnly` cookies if any are used.
- Apply the principle of least privilege to any stored PHI: encrypt sensitive columns at rest where supported by the datastore, and scope database credentials to the minimum required permissions.

## Input validation (OWASP Top 10)

- Validate and bound all external input (headers, query/path params, FHIR resource content) before use; reject malformed FHIR resources with an `OperationOutcome` rather than letting parsing exceptions leak stack traces.
- Use parameterized queries/EF Core LINQ exclusively — never string-concatenate SQL.
- Set explicit CORS policies per API (no `AllowAnyOrigin` combined with credentials).
- Rate-limit and validate payload size on public endpoints to mitigate DoS/resource-exhaustion.

## Dependencies

- Keep auth-related NuGet packages (`Microsoft.AspNetCore.Authentication.JwtBearer`, Firely SDK, etc.) up to date; treat security advisories on these as high priority.
