You are implementing a production-grade HL7 Da Vinci Electronic Prior Authorization (ePA) platform.

The repository already contains:

- .github/copilot-instructions.md
- .github/instructions/*
- AGENTS.md
- README.md

Use those files as the authoritative source for architecture, coding standards, folder responsibilities, technology choices, and implementation guidelines.

Do NOT repeat or ignore those instructions.

========================================================
OBJECTIVE
========================================================

Complete the implementation of this repository by adding production-ready code to the existing project structure.

Do NOT change the solution structure.

Do NOT rename projects.

Only create or modify files that are required.

Current solution:

DavinciEPA.sln

src/

├── DavinciEPA.CRD.Api
├── DavinciEPA.DTR.Api
├── DavinciEPA.PAS.Api
├── DavinciEPA.Core
├── DavinciEPA.Fhir
├── DavinciEPA.Infrastructure
├── DavinciEPA.Rules
├── DavinciEPA.Security
└── DavinciEPA.Shared

tests/

├── CRD.Tests
├── DTR.Tests
├── PAS.Tests
└── Integration.Tests

========================================================
IMPLEMENTATION ORDER
========================================================

Build the project in the following order.

Phase 1

✔ Core

Create

• Entities
• DTOs
• Interfaces
• Enums
• Constants
• Exceptions
• Validation
• Result Models

Phase 2

✔ Shared

Create

• Middleware
• Extension Methods
• Response Models
• Utility Classes

Phase 3

✔ Security

Create

• JWT Authentication
• OAuth2 Configuration
• Authorization Policies
• SMART on FHIR foundation
• Authentication Extensions

Phase 4

✔ FHIR

Create

• Firely SDK configuration
• FHIR Builders
• Resource Mapping
• Validators
• Serialization Helpers
• Bundle Builder
• OperationOutcome Builder

Phase 5

✔ Rules

Create

• Prior Authorization Rule Engine
• Coverage Rule Engine
• Medical Necessity Rule Engine
• Decision Models

Rules must never be hardcoded inside controllers.

Phase 6

✔ Infrastructure

Create

• DbContext
• EF Core Configurations
• Repositories
• SQL Server Integration
• Logging
• Configuration
• Unit Of Work

Phase 7

✔ CRD

Implement

Coverage Requirements Discovery

Endpoints

POST /cds-services/order-select

POST /cds-services/order-sign

Implement CDS Hooks cards.

Implement Prior Authorization determination.

Implement documentation requirement discovery.

Phase 8

✔ DTR

Implement

Questionnaire retrieval

Questionnaire Package

QuestionnaireResponse

Auto population

Validation

FHIR Questionnaire support

Phase 9

✔ PAS

Implement

Prior Authorization submission

FHIR Claim

FHIR Bundle

FHIR ClaimResponse

Status API

Authorization Tracking

Approval

Denial

Pending

Additional Information Required

Cancellation

Phase 10

✔ Tests

Generate

Unit Tests

Integration Tests

FHIR Validation Tests

========================================================
RULES
========================================================

Never generate placeholder code.

Never generate TODO comments.

Never generate pseudo code.

Never skip implementations.

Never duplicate logic.

Always reuse interfaces.

Always use dependency injection.

Always use async/await.

Always use nullable reference types.

Controllers must remain thin.

Business logic belongs in Services.

FHIR logic belongs in DavinciEPA.Fhir.

Repositories belong in Infrastructure.

Rule evaluation belongs in DavinciEPA.Rules.

Return OperationOutcome for FHIR errors.

Generate XML documentation for public APIs.

========================================================
HOW TO WORK
========================================================

Work incrementally.

Before generating code:

1. Analyze the existing solution.
2. Determine missing files.
3. Determine dependencies.
4. Explain your implementation plan.

Then:

Generate ONLY ONE FILE.

Wait for approval.

Continue with the next file.

Never generate more than one file per response.

If an existing file must be modified, explain why before modifying it.

Continue until the repository is fully implemented.