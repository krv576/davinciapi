# Da Vinci ePA Repository Instructions

## Project Overview

This repository implements a production-grade Da Vinci Electronic Prior Authorization (ePA) platform using ASP.NET Core .NET 8.

This is NOT a tutorial project.

Write code as if it will be deployed by a large healthcare payer.

Supported standards include:

- HL7 FHIR R4
- Da Vinci CRD
- Da Vinci DTR
- Da Vinci PAS
- CDS Hooks
- SMART on FHIR
- OAuth2
- OpenID Connect
- CMS-0057-F

---

# Technology Stack

Framework

- .NET 8
- ASP.NET Core Minimal API

FHIR

- Firely .NET SDK
- HL7 FHIR R4

Database

- SQL Server
- Entity Framework Core

Authentication

- OAuth2
- JWT
- SMART on FHIR

Testing

- xUnit
- FluentAssertions

Logging

- Serilog

Documentation

- Swagger / OpenAPI

---

# Architecture

Always follow Clean Architecture.

Never place business logic inside:

- Controllers
- API Endpoints
- Middleware

Business logic belongs inside Services and Rule Engine.

Repositories should never contain business logic.

Controllers should only:

- Validate requests
- Call services
- Return responses

---

# Folder Responsibilities

DavinciEPA.Core

Contains

- Entities
- DTOs
- Interfaces
- Constants
- Enums
- Exceptions
- Validation

DavinciEPA.Fhir

Contains

- FHIR Builders
- Parsers
- Validators
- Extensions
- Resource Mapping

DavinciEPA.Rules

Contains

- Coverage Rules
- Prior Authorization Rules
- Medical Necessity Rules

DavinciEPA.Infrastructure

Contains

- EF Core
- SQL Server
- Repositories
- External APIs
- Configuration
- Logging

DavinciEPA.Security

Contains

- OAuth
- JWT
- SMART Authentication
- Authorization Policies

DavinciEPA.Shared

Contains

- Middleware
- Helpers
- Shared Responses
- Utility Classes

CRD.Api

Contains Coverage Requirements Discovery endpoints.

DTR.Api

Contains Documentation Templates and Rules.

PAS.Api

Contains Prior Authorization submission endpoints.

---

# Coding Standards

Always

- Use nullable reference types
- Use async/await
- Use constructor injection
- Use dependency injection
- Use SOLID principles
- Keep classes focused on one responsibility
- Prefer composition over inheritance
- Use immutable DTOs where appropriate

Never

- Use static business classes
- Duplicate logic
- Hardcode configuration
- Hardcode connection strings
- Hardcode payer rules

---

# FHIR

Always use official Firely SDK models when available.

Never invent custom FHIR resources.

Validate resources before serialization.

Return OperationOutcome for FHIR validation failures.

---

# API Standards

Every endpoint should

- Validate input
- Return proper HTTP status codes
- Return OperationOutcome for FHIR errors
- Produce Swagger documentation

---

# Database

Use Entity Framework Core.

Use migrations.

Never use raw SQL unless performance requires it.

Repositories should expose interfaces.

---

# Security

Never disable authentication.

Use OAuth2.

Use JWT Bearer authentication.

Never log PHI or secrets.

---

# Testing

Every service should have unit tests.

Every endpoint should have integration tests.

Mock external systems.

Do not mock business logic.

---

# GitHub Copilot Behavior

When generating code:

Generate ONE file only.

Never generate multiple files.

Do not create placeholders.

Do not write TODO comments.

Generate production-quality code.

If dependencies are missing, explain them before generating code.

Do not modify unrelated files.

Wait for confirmation before generating the next file.