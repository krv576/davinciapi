# AGENTS.md

## Purpose

This repository contains a production-grade implementation of the HL7 Da Vinci Electronic Prior Authorization platform.

Agents working inside this repository should behave like experienced healthcare interoperability engineers.

---

# Primary Goal

Produce maintainable, secure, production-ready code.

Correctness is more important than speed.

FHIR compliance is more important than convenience.

---

# Development Workflow

Before writing code

1. Understand the feature.
2. Identify affected projects.
3. Check existing interfaces.
4. Reuse existing abstractions.
5. Avoid duplicate implementations.

---

# Code Generation Rules

Generate ONE file at a time.

Never generate an entire project.

Never create placeholder methods.

Never generate unfinished code.

Never leave TODO comments.

If information is missing, explain what is required instead of inventing behavior.

---

# Architecture Rules

Follow Clean Architecture.

Business rules belong in:

- DavinciEPA.Rules

FHIR logic belongs in:

- DavinciEPA.Fhir

Database logic belongs in:

- DavinciEPA.Infrastructure

Shared utilities belong in:

- DavinciEPA.Shared

Security belongs in:

- DavinciEPA.Security

API projects should remain thin.

---

# FHIR Rules

Use Firely SDK models.

Prefer official resources.

Use:

- Patient
- Coverage
- Organization
- Practitioner
- Encounter
- Condition
- Observation
- Questionnaire
- QuestionnaireResponse
- Claim
- ClaimResponse
- Bundle
- Task
- OperationOutcome

Never replace these with custom models unless there is a mapping layer.

---

# Error Handling

Always

- Validate inputs
- Throw meaningful exceptions
- Log errors
- Return OperationOutcome when required

Never swallow exceptions.

---

# Database Rules

Use EF Core.

Use repository pattern.

Create migrations.

Avoid raw SQL.

---

# Security Rules

Never expose PHI.

Never log JWT tokens.

Never hardcode secrets.

Use configuration.

---

# Pull Request Checklist

Before considering work complete

- Build succeeds
- Tests pass
- Swagger works
- Nullable warnings resolved
- No duplicated code
- Dependency injection configured
- Logging included
- XML comments added for public APIs

---

# Preferred Development Order

1. Core

2. Shared

3. Security

4. FHIR

5. Rules

6. Infrastructure

7. CRD

8. DTR

9. PAS

10. Tests

---

# Response Style

When generating code

Always explain

- Why the file exists
- Where it belongs
- Dependencies

Then generate the complete file.

Stop after one file.