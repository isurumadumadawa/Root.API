# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: [MUST be .NET 10]  
**Primary Dependencies**: [ASP.NET Core, MediatR, EF Core provider for PostgreSQL, Swagger/OpenAPI]  
**Storage**: [MUST be PostgreSQL via EF Core]  
**Testing**: [N/A - automated tests are prohibited by constitution unless formally amended]  
**Target Platform**: [e.g., Linux server, iOS 15+, WASM or NEEDS CLARIFICATION]
**Project Type**: [e.g., library/cli/web-service/mobile-app/compiler/desktop-app or NEEDS CLARIFICATION]  
**Performance Goals**: [domain-specific, e.g., 1000 req/s, 10k lines/sec, 60 fps or NEEDS CLARIFICATION]  
**Constraints**: [domain-specific, e.g., <200ms p95, <100MB memory, offline-capable or NEEDS CLARIFICATION]  
**Scale/Scope**: [domain-specific, e.g., 10k users, 1M LOC, 50 screens or NEEDS CLARIFICATION]

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Clean Architecture boundaries are explicit (API/Application/Domain/Infrastructure) and dependency direction is inward only.
- Endpoint contracts include complete Swagger request/response schemas and documented error shapes.
- Persistence design uses PostgreSQL and EF Core migrations.
- Application flow uses MediatR requests/handlers.
- Validation, centralized error handling, and crucial-step structured logging are explicitly planned.
- No unit/integration/e2e/contract/other automated tests are included.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
Root.API/
├── Controllers/                 # API layer
├── Application/                 # Use cases, MediatR requests/handlers
├── Domain/                      # Entities and business rules
├── Infrastructure/              # EF Core, PostgreSQL, integrations
├── Contracts/                   # Request/response DTOs and schema contracts
└── Program.cs                   # Composition root (middleware, Swagger, logging)
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., temporary architecture shortcut] | [current need] | [why constitution-compliant approach was not possible now] |
