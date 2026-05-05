# Tasks: User Role Management API

**Input**: Design documents from `/specs/001-user-role-management/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/openapi.yaml, quickstart.md

**Tests**: Do not add test tasks. Unit, integration, end-to-end, contract, and any other automated testing are out of scope by constitution and feature requirements.

**Organization**: Tasks are grouped by user story so each story can be implemented and validated independently through Swagger.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Task can run in parallel (different files, no unfinished dependency)
- **[Story]**: User story label (US1, US2, US3, US4)
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare project structure and baseline dependencies

- [X] T001 Create Clean Architecture folders and placeholder files in `Root.API/Application/.gitkeep`, `Root.API/Domain/.gitkeep`, `Root.API/Infrastructure/.gitkeep`, and `Root.API/Contracts/.gitkeep`
- [X] T002 Remove template sample API files `Root.API/Controllers/WeatherForecastController.cs` and `Root.API/WeatherForecast.cs`
- [X] T003 Update package references for EF Core PostgreSQL, MediatR, FluentValidation, JwtBearer, and Swashbuckle in `Root.API/Root.API.csproj`
- [X] T004 [P] Add PostgreSQL and JWT settings sections with spec defaults in `Root.API/appsettings.json`
- [X] T005 [P] Add development overrides for PostgreSQL and logging in `Root.API/appsettings.Development.json`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core platform plumbing required before user story implementation

**CRITICAL**: Complete this phase before starting any user story

- [X] T006 Define domain enums and entities for role, user status, and user in `Root.API/Domain/Enums/RoleName.cs`, `Root.API/Domain/Enums/UserStatus.cs`, and `Root.API/Domain/Entities/User.cs`
- [X] T007 [P] Define role entity and domain constants for fixed role seeds in `Root.API/Domain/Entities/Role.cs` and `Root.API/Domain/Constants/RoleSeeds.cs`
- [X] T008 Create EF Core DbContext and model configuration for users/roles and unique username in `Root.API/Infrastructure/Persistence/ApplicationDbContext.cs`
- [X] T009 [P] Add entity type configurations for user and role mappings in `Root.API/Infrastructure/Persistence/Configurations/UserConfiguration.cs` and `Root.API/Infrastructure/Persistence/Configurations/RoleConfiguration.cs`
- [X] T010 Implement password hashing and token service abstractions/implementations in `Root.API/Application/Abstractions/IPasswordHasher.cs`, `Root.API/Application/Abstractions/ITokenService.cs`, `Root.API/Infrastructure/Security/BcryptPasswordHasher.cs`, and `Root.API/Infrastructure/Security/JwtTokenService.cs`
- [X] T011 Implement standardized API error contract models in `Root.API/Contracts/Responses/ErrorResponse.cs` and `Root.API/Contracts/Responses/ValidationErrorItem.cs`
- [X] T012 Implement global exception middleware and exception types in `Root.API/API/Middleware/ExceptionHandlingMiddleware.cs`, `Root.API/Application/Common/Exceptions/DomainException.cs`, and `Root.API/Application/Common/Exceptions/ForbiddenException.cs`
- [X] T013 Implement validation pipeline behavior and base request contracts in `Root.API/Application/Common/Behaviors/ValidationBehavior.cs` and `Root.API/Application/Common/Abstractions/ICurrentUserContext.cs`
- [X] T014 [P] Implement request logging behavior with correlation metadata in `Root.API/Application/Common/Behaviors/RequestLoggingBehavior.cs`
- [X] T015 Configure DI, authentication/authorization, middleware pipeline, Swagger security, and OpenAPI setup in `Root.API/Program.cs`
- [X] T016 Add EF Core migration for initial schema in `Root.API/Infrastructure/Persistence/Migrations/InitialCreate.cs` and `Root.API/Infrastructure/Persistence/Migrations/InitialCreate.Designer.cs`
- [X] T017 Add startup seeding logic for roles and default admin (`Root Admin` / `123@Admin` hashed) in `Root.API/Infrastructure/Persistence/Seed/DatabaseSeeder.cs`
- [X] T018 Integrate migration + seed execution during startup in `Root.API/Program.cs`

**Checkpoint**: Foundation is ready for user story delivery

---

## Phase 3: User Story 1 - Authentication and Role Resolution (Priority: P1) 🎯 MVP

**Goal**: Users can sign in, receive token, and resolve role via token-protected endpoint

**Independent Validation**: In Swagger, sign in with valid credentials to get token, call role endpoint with bearer token, and verify role is returned; invalid token returns unauthorized payload.

- [X] T019 [P] [US1] Create auth request/response DTOs in `Root.API/Contracts/Requests/Auth/SignInRequest.cs`, `Root.API/Contracts/Responses/Auth/SignInResponse.cs`, and `Root.API/Contracts/Responses/Auth/RoleResponse.cs`
- [X] T020 [P] [US1] Implement sign-in command/query models in `Root.API/Application/Authentication/Commands/SignInCommand.cs` and `Root.API/Application/Authentication/Queries/GetMyRoleQuery.cs`
- [X] T021 [US1] Implement auth handlers for sign-in and role lookup in `Root.API/Application/Authentication/Handlers/SignInCommandHandler.cs` and `Root.API/Application/Authentication/Handlers/GetMyRoleQueryHandler.cs`
- [X] T022 [US1] Implement auth validators for sign-in payload in `Root.API/Application/Authentication/Validators/SignInRequestValidator.cs`
- [X] T023 [US1] Implement auth controller endpoints `/auth/sign-in` and `/auth/role` in `Root.API/Controllers/AuthController.cs`
- [X] T024 [US1] Add Swagger response annotations and error schemas for auth endpoints in `Root.API/Controllers/AuthController.cs`
- [X] T025 [US1] Add crucial-step logs for auth attempts/outcomes and role resolution in `Root.API/Application/Authentication/Handlers/SignInCommandHandler.cs` and `Root.API/Application/Authentication/Handlers/GetMyRoleQueryHandler.cs`

**Checkpoint**: User Story 1 is independently functional and validatable in Swagger

---

## Phase 4: User Story 2 - Admin User Lifecycle Control (Priority: P1)

**Goal**: Admin can create, update, soft-delete, and read any user (including deleted)

**Independent Validation**: In Swagger with admin token, create user, update user, fetch list/detail, soft-delete user; deleted user cannot sign in; non-admin receives forbidden.

- [X] T026 [P] [US2] Create admin user management DTOs in `Root.API/Contracts/Requests/Users/CreateUserRequest.cs`, `Root.API/Contracts/Requests/Users/AdminUpdateUserRequest.cs`, `Root.API/Contracts/Responses/Users/UserDetailResponse.cs`, and `Root.API/Contracts/Responses/Users/DeleteUserResponse.cs`
- [X] T027 [P] [US2] Implement admin commands and queries in `Root.API/Application/Users/Commands/CreateUserCommand.cs`, `Root.API/Application/Users/Commands/AdminUpdateUserCommand.cs`, `Root.API/Application/Users/Commands/SoftDeleteUserCommand.cs`, `Root.API/Application/Users/Queries/GetUsersQuery.cs`, and `Root.API/Application/Users/Queries/GetUserByIdQuery.cs`
- [X] T028 [US2] Implement admin handlers enforcing admin-only permissions and soft-delete semantics in `Root.API/Application/Users/Handlers/CreateUserCommandHandler.cs`, `Root.API/Application/Users/Handlers/AdminUpdateUserCommandHandler.cs`, and `Root.API/Application/Users/Handlers/SoftDeleteUserCommandHandler.cs`
- [X] T029 [US2] Implement validators for create/admin update requests including username conflict and role constraints in `Root.API/Application/Users/Validators/CreateUserRequestValidator.cs` and `Root.API/Application/Users/Validators/AdminUpdateUserRequestValidator.cs`
- [X] T030 [US2] Implement admin endpoints for create/list/detail/update/delete in `Root.API/Controllers/UsersController.cs`
- [X] T031 [US2] Add authorization policies/attributes for admin-only write operations in `Root.API/Program.cs` and `Root.API/Controllers/UsersController.cs`
- [X] T032 [US2] Add Swagger schemas and response codes for admin lifecycle endpoints in `Root.API/Controllers/UsersController.cs`
- [X] T033 [US2] Add crucial-step logs for create/update/soft-delete and authorization denials in `Root.API/Application/Users/Handlers/CreateUserCommandHandler.cs`, `Root.API/Application/Users/Handlers/AdminUpdateUserCommandHandler.cs`, and `Root.API/Application/Users/Handlers/SoftDeleteUserCommandHandler.cs`

**Checkpoint**: User Story 2 is independently functional and validatable in Swagger

---

## Phase 5: User Story 3 - User Self-Service Profile Access (Priority: P2)

**Goal**: Standard user can read own profile and update only name/position

**Independent Validation**: In Swagger with user token, self read and self update succeed; attempts to update username/role/createdDate or other users fail with documented errors.

- [X] T034 [P] [US3] Create self-service DTOs for self update and self detail in `Root.API/Contracts/Requests/Users/SelfUpdateUserRequest.cs` and `Root.API/Contracts/Responses/Users/UserSelfDetailResponse.cs`
- [X] T035 [P] [US3] Implement self-service query/command models in `Root.API/Application/Users/Queries/GetMyProfileQuery.cs` and `Root.API/Application/Users/Commands/UpdateMyProfileCommand.cs`
- [X] T036 [US3] Implement self-service handlers enforcing immutable fields and self-only access in `Root.API/Application/Users/Handlers/GetMyProfileQueryHandler.cs` and `Root.API/Application/Users/Handlers/UpdateMyProfileCommandHandler.cs`
- [X] T037 [US3] Implement validator for self update payload (name/position only) in `Root.API/Application/Users/Validators/SelfUpdateUserRequestValidator.cs`
- [X] T038 [US3] Add self endpoints `/users/me` get and put in `Root.API/Controllers/UsersController.cs`
- [X] T039 [US3] Add Swagger schemas and error responses for self-service endpoints in `Root.API/Controllers/UsersController.cs`
- [X] T040 [US3] Add logs for self read/update operations and immutable-field rejection paths in `Root.API/Application/Users/Handlers/UpdateMyProfileCommandHandler.cs`

**Checkpoint**: User Story 3 is independently functional and validatable in Swagger

---

## Phase 6: User Story 4 - Agent Read-Only User Visibility (Priority: P2)

**Goal**: Agent can read all or single users (including deleted) and cannot modify data

**Independent Validation**: In Swagger with agent token, list/detail read succeeds with status flags including deleted users; create/update/delete endpoints return forbidden.

- [X] T041 [P] [US4] Add agent read query options/DTO projection updates for status visibility in `Root.API/Application/Users/Queries/GetUsersQuery.cs`, `Root.API/Application/Users/Queries/GetUserByIdQuery.cs`, and `Root.API/Contracts/Responses/Users/UserSummaryResponse.cs`
- [X] T042 [US4] Update read handlers to permit admin/agent visibility for deleted users with explicit status mapping in `Root.API/Application/Users/Handlers/GetUsersQueryHandler.cs` and `Root.API/Application/Users/Handlers/GetUserByIdQueryHandler.cs`
- [X] T043 [US4] Add authorization policy allowing agent/admin read routes and denying write routes in `Root.API/Program.cs` and `Root.API/Controllers/UsersController.cs`
- [X] T044 [US4] Update Swagger documentation for agent role visibility and forbidden write responses in `Root.API/Controllers/UsersController.cs`
- [X] T045 [US4] Add read-path logging for agent access and denied write attempts in `Root.API/Application/Users/Handlers/GetUsersQueryHandler.cs`, `Root.API/Application/Users/Handlers/GetUserByIdQueryHandler.cs`, and `Root.API/API/Middleware/ExceptionHandlingMiddleware.cs`

**Checkpoint**: User Story 4 is independently functional and validatable in Swagger

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Finalize consistency, docs, and manual validation flow

- [X] T046 [P] Update OpenAPI contract to match implemented endpoints and schema details in `specs/001-user-role-management/contracts/openapi.yaml`
- [X] T047 Sync manual validation checklist with final endpoint behavior in `specs/001-user-role-management/quickstart.md`
- [X] T048 Harden configuration and secret handling notes for deployment in `Root.API/appsettings.json` and `specs/001-user-role-management/quickstart.md`
- [X] T049 [P] Refine ProblemDetails-style error examples in contracts and controller annotations in `specs/001-user-role-management/contracts/openapi.yaml`, `Root.API/Controllers/AuthController.cs`, and `Root.API/Controllers/UsersController.cs`
- [X] T050 Verify build/run commands and startup prerequisites in `specs/001-user-role-management/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: Starts immediately.
- **Phase 2 (Foundational)**: Depends on Phase 1 and blocks all user stories.
- **Phase 3 (US1)**: Depends on Phase 2; this is MVP.
- **Phase 4 (US2)**: Depends on Phase 2; can run in parallel with US1 after shared auth components exist.
- **Phase 5 (US3)**: Depends on Phase 2 and user/auth components from US1.
- **Phase 6 (US4)**: Depends on Phase 2 and user read models from US2.
- **Phase 7 (Polish)**: Depends on completion of selected user stories.

### User Story Dependencies

- **US1 (P1)**: Independent after foundation.
- **US2 (P1)**: Independent after foundation; shares auth and user entities.
- **US3 (P2)**: Depends on auth identity flow from US1 and user model from US2.
- **US4 (P2)**: Depends on user read model and role policies established in US2.

### Within Each User Story

- DTOs and request models before handlers.
- Handlers before controller endpoint wiring.
- Validation and authorization checks before Swagger finalization.
- Logging must be completed for crucial paths before story checkpoint.

---

## Parallel Opportunities

- **Setup**: T004 and T005 can run in parallel after T003.
- **Foundational**: T007, T009, and T014 can run in parallel once entity skeletons exist.
- **US1**: T019 and T020 can run in parallel, then converge at T021.
- **US2**: T026 and T027 can run in parallel, then converge at T028.
- **US3**: T034 and T035 can run in parallel, then converge at T036.
- **US4**: T041 can proceed while policy updates (T043) are prepared.
- **Polish**: T046 and T049 can run in parallel.

## Parallel Example: User Story 2

```bash
# Parallelizable US2 work after foundation:
Task: "T026 [P] [US2] Create admin user management DTOs in Root.API/Contracts/Requests/Users/CreateUserRequest.cs, Root.API/Contracts/Requests/Users/AdminUpdateUserRequest.cs, Root.API/Contracts/Responses/Users/UserDetailResponse.cs, and Root.API/Contracts/Responses/Users/DeleteUserResponse.cs"
Task: "T027 [P] [US2] Implement admin commands and queries in Root.API/Application/Users/Commands/CreateUserCommand.cs, Root.API/Application/Users/Commands/AdminUpdateUserCommand.cs, Root.API/Application/Users/Commands/SoftDeleteUserCommand.cs, Root.API/Application/Users/Queries/GetUsersQuery.cs, and Root.API/Application/Users/Queries/GetUserByIdQuery.cs"
```

---

## Implementation Strategy

### MVP First (US1)

1. Complete Setup and Foundational phases.
2. Deliver User Story 1 (sign-in + role endpoint).
3. Validate in Swagger and confirm non-expiring token behavior.

### Incremental Delivery

1. Add US2 admin lifecycle controls.
2. Add US3 user self-service constraints.
3. Add US4 agent read-only visibility.
4. Finish with cross-cutting polish and docs sync.

### Team Parallel Strategy

1. Engineer A: Foundation + auth pipeline.
2. Engineer B: User DTOs and admin lifecycle handlers.
3. Engineer C: Swagger contract sync + middleware/error contracts.

---

## Notes

- All tasks follow required checklist format: `- [ ] T### [P] [US#] Description with file path`.
- No automated test tasks are included.
- Validate each story independently via Swagger using `specs/001-user-role-management/quickstart.md`.
- Keep Clean Architecture dependency flow inward (API -> Application -> Domain; Infrastructure depends on Application/Domain abstractions).

