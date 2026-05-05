<!--
Sync Impact Report
- Version change: N/A -> 1.0.0
- Modified principles:
	- N/A -> I. .NET 10 Clean Architecture Mandate
	- N/A -> II. API Contract Clarity with Swagger
	- N/A -> III. PostgreSQL + EF Core Persistence Standard
	- N/A -> IV. Endpoint Validation and Bug-Resilient Errors
	- N/A -> V. MediatR Flow and Crucial-Step Logging
- Added sections:
	- Technology Guardrails
	- Delivery Workflow
- Removed sections:
	- None
- Templates requiring updates:
	- ✅ updated .specify/templates/plan-template.md
	- ✅ updated .specify/templates/spec-template.md
	- ✅ updated .specify/templates/tasks-template.md
	- ⚠ pending .specify/templates/commands/*.md (folder not present)
- Deferred TODOs:
	- None
-->

# Root.API Constitution

## Core Principles

### I. .NET 10 Clean Architecture Mandate
All implementation MUST target .NET 10 and follow Clean Architecture boundaries:
API (presentation), Application (use cases), Domain (business rules), and
Infrastructure (external concerns). Dependencies MUST point inward only.
Domain MUST remain framework-agnostic.

Rationale: Boundary discipline prevents feature coupling and reduces regression
risk when requirements or infrastructure evolve.

### II. API Contract Clarity with Swagger
Every endpoint MUST be exposed and runnable through Swagger/OpenAPI with
complete request and response schemas, including validation constraints,
required fields, response codes, and error payload shapes.

Rationale: Fully defined contracts reduce integration ambiguity and make API
behavior inspectable before client implementation.

### III. PostgreSQL + EF Core Persistence Standard
Persistent storage MUST use PostgreSQL through EF Core. Data access MUST be
implemented via Infrastructure-layer repositories or data services mapped from
Application abstractions. Schema changes MUST be managed by EF Core migrations.

Rationale: A single persistence standard simplifies operations, migration
planning, and developer onboarding.

### IV. Endpoint Validation and Bug-Resilient Errors
Every endpoint MUST apply explicit input validation before handler execution.
Each endpoint MUST return deterministic, documented error responses for
validation failures and unexpected exceptions. Bug-prone paths MUST be handled
through centralized exception handling and consistent problem details payloads.

Rationale: Early validation and uniform error semantics reduce production
defects and improve client-side recoverability.

### V. MediatR Flow and Crucial-Step Logging
Application request handling MUST follow the MediatR pattern for commands,
queries, and notifications. Crucial processing steps MUST emit structured logs
with correlation data at ingress, business decision points, persistence
boundaries, and failure exits.

Rationale: Mediated request flow standardizes orchestration, while structured
logging improves diagnosis and auditability.

## Technology Guardrails

- Testing is explicitly out of scope: no unit, integration, end-to-end,
	contract, or any other automated tests MUST be added unless this
	constitution is amended.
- API development MUST prioritize production-ready endpoints, complete Swagger
	documentation, robust validation, and clear operational logs.
- New dependencies MUST be justified against the existing .NET 10, MediatR,
	EF Core, and PostgreSQL baseline.

## Delivery Workflow

- Feature specs and plans MUST include architecture-layer placement,
	MediatR request/handler mapping, and endpoint-level validation and error
	behavior.
- Task lists MUST emphasize endpoint delivery, schema completeness, migrations,
	logging, and error handling; test-task generation is prohibited by default.
- Reviews MUST block changes that violate Clean Architecture direction,
	omit Swagger schemas, skip validation, or omit crucial-step logs.

## Governance

This constitution overrides conflicting local templates and workflow defaults.
Amendments require: (1) a documented change proposal, (2) explicit approval by
project maintainers, and (3) synchronized updates to dependent templates.

Versioning policy for this document uses semantic versioning:
- MAJOR for backward-incompatible governance changes or principle removals.
- MINOR for new principles/sections or materially expanded obligations.
- PATCH for wording clarifications and non-semantic refinements.

Compliance review is required during specification, planning, and task
generation. Any exception MUST include a written rationale and an amendment
plan.

**Version**: 1.0.0 | **Ratified**: 2026-05-05 | **Last Amended**: 2026-05-05
