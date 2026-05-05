# Research: User Role Management API

## Decision 1: Token format and non-expiry behavior

- Decision: Use JWT Bearer tokens signed with a symmetric key, without automatic expiry claim enforcement (no `exp` claim validation requirement for this feature).
- Rationale: ASP.NET Core supports JWT middleware natively, integrates with Swagger auth flows, and allows claim-based authorization with role claim lookup endpoint requirements.
- Alternatives considered:
  - Opaque reference tokens in database: rejected due to added persistence and revocation complexity outside this feature scope.
  - Cookie-based auth: rejected because API-first Swagger workflow and token transport requirements favor Bearer tokens.

## Decision 2: Role and admin bootstrap strategy

- Decision: Seed the `Roles` table with exactly `user`, `admin`, and `agent`, and seed a default `Root Admin` account during migration/startup initialization.
- Rationale: EF Core migration and startup initialization patterns are deterministic and match constitution mandates for migration-based schema control.
- Alternatives considered:
  - Manual SQL seeding outside app: rejected due to drift risk and reduced reproducibility.
  - Runtime creation on first login only: rejected because roles/admin must exist before API usage.

## Decision 3: Soft delete and read visibility model

- Decision: Implement soft delete via status flags (`isDeleted`, `deletedAtUtc`) and preserve records in all admin/agent read endpoints with explicit status fields.
- Rationale: Aligns with clarified requirements while preserving auditability and role-specific visibility.
- Alternatives considered:
  - Hard delete: rejected by clarified requirement.
  - Global query filter hiding deleted users: rejected because admin/agent must always see deleted users.

## Decision 4: Validation, authorization errors, and exception handling

- Decision: Use request validators (FluentValidation or equivalent), map failures to a standardized ProblemDetails-like schema, and centralize unexpected exception mapping in middleware.
- Rationale: Meets constitution requirements for deterministic, documented error responses and reduces duplicated endpoint error logic.
- Alternatives considered:
  - Controller-level manual validation only: rejected due to duplication and inconsistency risks.
  - Handler-level ad hoc error payloads: rejected due to Swagger contract fragmentation.

## Decision 5: MediatR architecture and logging

- Decision: Use MediatR command/query handlers for all endpoint operations plus pipeline behaviors for validation and request logging with correlation data.
- Rationale: Satisfies constitution MediatR mandate and creates consistent request orchestration and observability.
- Alternatives considered:
  - Fat controllers calling repositories directly: rejected due to Clean Architecture and maintainability concerns.
  - Service-layer orchestration without MediatR: rejected by constitutional principle V.

## Decision 6: PostgreSQL configuration and migration approach

- Decision: Bind PostgreSQL connection settings from environment variables with defaults from spec, and apply EF Core migrations during deployment/bootstrap.
- Rationale: Preserves required defaults while allowing environment overrides and consistent schema evolution.
- Alternatives considered:
  - Hardcoded connection string only: rejected due to environment portability limits.
  - Auto-create schema without migrations: rejected by constitution and upgrade traceability concerns.

## Decision 7: Swagger schema completeness enforcement

- Decision: Define explicit request/response DTOs for every endpoint and annotate all expected response codes (success, validation, unauthorized, forbidden, not found, conflict).
- Rationale: Directly satisfies FR-018 and provides verifiable client-facing contracts.
- Alternatives considered:
  - Implicit schema inference from domain entities: rejected due to accidental overexposure and inconsistent docs.
  - Partial response documentation: rejected by feature success criteria.
