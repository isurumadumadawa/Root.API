---

description: "Task list template for feature implementation"
---

# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Do not generate any test tasks. Unit, integration, end-to-end, contract, or any automated testing tasks are prohibited unless the constitution is amended.

**Organization**: Tasks are grouped by user story to enable independent implementation and independent endpoint validation via Swagger.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Single project**: `src/`, `tests/` at repository root
- **Web app**: `backend/src/`, `frontend/src/`
- **Mobile**: `api/src/`, `ios/src/` or `android/src/`
- Paths shown below assume single project - adjust based on plan.md structure

<!-- 
  ============================================================================
  IMPORTANT: The tasks below are SAMPLE TASKS for illustration purposes only.
  
  The /speckit.tasks command MUST replace these with actual tasks based on:
  - User stories from spec.md (with their priorities P1, P2, P3...)
  - Feature requirements from plan.md
  - Entities from data-model.md
  - Endpoints from contracts/
  
  Tasks MUST be organized by user story so each story can be:
  - Implemented independently
  - Validated independently through Swagger execution
  - Delivered as an MVP increment
  
  DO NOT keep these sample tasks in the generated tasks.md file.
  ============================================================================
-->

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create Clean Architecture folders/layers per implementation plan
- [ ] T002 Initialize .NET 10 Web API dependencies (MediatR, EF Core, Npgsql, Swagger)
- [ ] T003 [P] Configure baseline structured logging and correlation IDs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

Examples of foundational tasks (adjust based on your project):

- [ ] T004 Setup database schema and migrations framework
- [ ] T005 [P] Configure PostgreSQL connectivity using EF Core provider
- [ ] T006 [P] Setup API routing, middleware, and centralized exception handling
- [ ] T007 Create base domain entities and application abstractions
- [ ] T008 Configure Swagger/OpenAPI with detailed request and response schemas
- [ ] T009 Setup environment configuration management

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - [Title] (Priority: P1) 🎯 MVP

**Goal**: [Brief description of what this story delivers]

**Independent Validation**: [How to verify this story in Swagger with expected status codes and payload schemas]

### Implementation for User Story 1

- [ ] T010 [P] [US1] Create command/query and handler with MediatR in Application layer
- [ ] T011 [P] [US1] Create/update domain entities and EF Core mappings
- [ ] T012 [US1] Implement [endpoint/feature] in API layer and wire MediatR
- [ ] T013 [US1] Add validation and standardized error responses
- [ ] T014 [US1] Add crucial-step structured logs for user story 1 flow
- [ ] T015 [US1] Update Swagger schemas and examples for user story 1

**Checkpoint**: At this point, User Story 1 should be fully functional and independently validated in Swagger

---

## Phase 4: User Story 2 - [Title] (Priority: P2)

**Goal**: [Brief description of what this story delivers]

**Independent Validation**: [How to verify this story in Swagger with expected status codes and payload schemas]

### Implementation for User Story 2

- [ ] T016 [P] [US2] Create command/query and handler with MediatR in Application layer
- [ ] T017 [US2] Implement [endpoint/feature] and integrate with prior shared components
- [ ] T018 [US2] Add validation/error handling behavior and logging
- [ ] T019 [US2] Update Swagger schemas and examples for user story 2

**Checkpoint**: At this point, User Stories 1 AND 2 should both be independently validated in Swagger

---

## Phase 5: User Story 3 - [Title] (Priority: P3)

**Goal**: [Brief description of what this story delivers]

**Independent Validation**: [How to verify this story in Swagger with expected status codes and payload schemas]

### Implementation for User Story 3

- [ ] T020 [P] [US3] Create command/query and handler with MediatR in Application layer
- [ ] T021 [US3] Implement [endpoint/feature] and persistence integration
- [ ] T022 [US3] Add validation/error handling behavior and logging
- [ ] T023 [US3] Update Swagger schemas and examples for user story 3

**Checkpoint**: All user stories should now be independently validated in Swagger

---

[Add more user story phases as needed, following the same pattern]

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] TXXX [P] Documentation updates in docs/
- [ ] TXXX Code cleanup and refactoring
- [ ] TXXX Performance optimization across all stories
- [ ] TXXX Security hardening
- [ ] TXXX Run quickstart.md validation

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1 but should be independently validatable in Swagger
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - May integrate with US1/US2 but should be independently validatable in Swagger

### Within Each User Story

- Models before services
- Services before endpoints
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- Models within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# Launch independent User Story 1 tasks together:
Task: "Create command/query and handler with MediatR in Application layer"
Task: "Create/update domain entities and EF Core mappings"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Validate User Story 1 independently in Swagger
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Validate independently in Swagger → Deploy/Demo (MVP!)
3. Add User Story 2 → Validate independently in Swagger → Deploy/Demo
4. Add User Story 3 → Validate independently in Swagger → Deploy/Demo
5. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and validatable via Swagger
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence, and any test-task generation
