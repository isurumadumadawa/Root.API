# Implementation Plan: User Role Management API

**Branch**: `001-run-feature-hook` | **Date**: 2026-05-05 | **Spec**: `/specs/001-user-role-management/spec.md`
**Input**: Feature specification from `/specs/001-user-role-management/spec.md`

## Summary

Deliver a role-based user management API on .NET 10 with strict Clean Architecture boundaries, MediatR request handling, and PostgreSQL persistence via EF Core migrations. The feature includes: seeded roles (`user`, `admin`, `agent`), default admin account bootstrap, sign-in token issuance (no automatic expiry), token-based role lookup, admin lifecycle controls (create/update/soft-delete/read), user self-service constraints, and agent read-only visibility. Swagger must fully describe all success/error schemas, with centralized validation, authorization error handling, and structured logs at critical processing steps.

## Technical Context

**Language/Version**: C# / .NET 10  
**Primary Dependencies**: ASP.NET Core Web API, MediatR, FluentValidation (or equivalent validator pipeline), EF Core 10 + Npgsql provider, Swashbuckle.AspNetCore/OpenAPI, Microsoft.AspNetCore.Authentication.JwtBearer  
**Storage**: PostgreSQL via EF Core migrations (connection from environment; default fallback values per spec)  
**Testing**: N/A - automated tests are prohibited by constitution unless formally amended  
**Target Platform**: ASP.NET Core Web API hosted on Kestrel (Windows dev, Linux-compatible deployment)  
**Project Type**: Web service (REST API with Swagger UI)  
**Performance Goals**: P95 < 300ms for single-user reads and role lookup under normal load; auth and CRUD endpoints remain interactive in Swagger (< 2s per request in local environment)  
**Constraints**: Non-expiring auth token, soft-delete behavior with status visibility, immutable self-update fields (username/role/createdDate), strict role authorization, complete Swagger schemas, centralized error payloads, structured logging, no automated tests  
**Scale/Scope**: Initial release targeting internal operations for up to 10k user records with three fixed roles and single-service API boundary

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Pre-Research Gate

- PASS: Clean Architecture boundaries are explicit (API/Application/Domain/Infrastructure) and dependency direction is inward only.
- PASS: Endpoint contracts are planned with complete Swagger request/response schemas, including validation and authorization error payloads.
- PASS: Persistence is PostgreSQL with EF Core migration-driven schema evolution.
- PASS: Application flow is MediatR command/query based.
- PASS: Validation, centralized exception mapping, and crucial-step structured logging are in scope for every endpoint.
- PASS: No automated tests are planned, matching constitution constraints.

### Post-Design Re-Check

- PASS: Data model keeps business rules in Domain and persistence mapping in Infrastructure.
- PASS: Contracts include endpoint payload definitions and consistent error shapes.
- PASS: Migration/seed strategy defined for roles and default admin bootstrap.
- PASS: MediatR command/query surface covers authentication, role lookup, and user lifecycle actions.
- PASS: Validation/error/logging concerns are included in quickstart and design artifacts.
- PASS: No test artifacts introduced.

## Project Structure

### Documentation (this feature)

```text
specs/001-user-role-management/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   `-- openapi.yaml
`-- tasks.md
```

### Source Code (repository root)

```text
Root.API/
|-- Program.cs
|-- Controllers/
|-- Application/
|   |-- Abstractions/
|   |-- Authentication/
|   |-- Users/
|   `-- Common/
|-- Domain/
|   |-- Entities/
|   |-- ValueObjects/
|   `-- Enums/
|-- Infrastructure/
|   |-- Persistence/
|   |-- Identity/
|   `-- Logging/
`-- Contracts/
    |-- Requests/
    `-- Responses/
```

**Structure Decision**: Use a single Web API host with internal Clean Architecture folders. Keep controllers thin and forward to MediatR. Domain holds business rules (role constraints, soft-delete invariants, immutable-field guardrails). Infrastructure owns EF Core DbContext, migrations, seed logic, and token generation implementation. Contracts folder contains transport DTOs used for Swagger schema completeness.

## Phase 0: Research Plan

- Research token strategy for non-expiring tokens in ASP.NET Core JWT and safe claim set design.
- Research EF Core + PostgreSQL seeding patterns for immutable role set and bootstrap admin account.
- Research MediatR pipeline behaviors for validation, authorization checks, and structured logging enrichment.
- Research Swagger patterns to guarantee explicit success/error schema coverage for every endpoint.

## Phase 1: Design Plan

- Produce data model for `User`, `Role`, and authentication token representation with lifecycle/state transitions.
- Define OpenAPI contract for authentication, role lookup, and role-gated user lifecycle endpoints.
- Create quickstart flow for local PostgreSQL configuration, migrations, role/admin seed verification, and Swagger-driven manual validation.
- Update agent context file to reference this plan for future coding steps.

## Complexity Tracking

No constitution violations identified. This section is intentionally empty.
