# Feature Specification: User Role Management API

**Feature Branch**: `001-run-feature-hook`  
**Created**: 2026-05-05  
**Status**: Draft  
**Input**: User description: "details, update user , delete user, there should be role table as well initialy role table should only have three role user, admin and agent. by default admin user should vreated with user name \"Root Admin\" and password \"123@Admin\" user signup permission should only have the admin. every created user should be able to sign in and in the sign in they should get the token. there should be one end point when request to the API with token should get the role. admin should be able to delete update any user detils. user role should have only to see there deails and update details only. agent should have access to see all user details or individual any user data user should have name user name created date position kind of data in the db

following are the db connection details

POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_DATABASE=root_memory
POSTGRES_USER=user
POSTGRES_PASSWORD=123456"

## Clarifications

### Session 2026-05-05

- Q: Should default admin credentials be forced to change on first login? -> A: Keep default admin credentials permanently valid with no forced password change.
- Q: Should user deletion be hard delete or soft delete? -> A: Soft delete user records (mark inactive/deleted) and block sign-in.
- Q: Should soft-deleted users appear in read endpoints? -> A: Always include soft-deleted users in admin/agent reads with a status flag.
- Q: Which fields can a user role update on own profile? -> A: User can update name and position only (username, role, created date immutable).
- Q: What should token expiry duration be? -> A: Token never expires.

## User Scenarios *(mandatory)*

### User Story 1 - Authentication and Role Resolution (Priority: P1)

As an authenticated platform user, I can sign in and receive a token, and I can call a token-based endpoint to retrieve my role so that access behavior is predictable and role-specific features can be enforced.

**Why this priority**: All role-based authorization and endpoint permissions depend on successful authentication and role resolution.

**Independent Validation**: Validate by signing in with a known account via Swagger, receiving a token, then calling the role endpoint with that token and confirming the returned role matches the account role.

**Acceptance Scenarios**:

1. **Given** a valid user account exists, **When** the user signs in with valid credentials, **Then** the API returns an authentication token and success response.
2. **Given** a valid token is provided, **When** the caller requests role resolution, **Then** the API returns the caller's assigned role.
3. **Given** an invalid token, **When** role resolution is requested, **Then** the API returns an unauthorized response with a documented error payload.

---

### User Story 2 - Admin User Lifecycle Control (Priority: P1)

As an admin, I can create users, update any user details, delete any user, and retrieve user records so that user lifecycle management remains centrally controlled.

**Why this priority**: The user explicitly requires signup to be admin-only and requires full admin control over user management operations.

**Independent Validation**: Validate by using an admin token in Swagger to create a user, update that user's details, retrieve the user, and delete the user while confirming expected responses.

**Acceptance Scenarios**:

1. **Given** an authenticated admin, **When** the admin creates a new user, **Then** the user record is persisted with role, name, username, created date, position, and status.
2. **Given** an authenticated admin, **When** the admin updates any user's details, **Then** the updated values are returned and persisted.
3. **Given** an authenticated admin, **When** the admin deletes any user, **Then** the user is soft-deleted (inactive/deleted) and cannot sign in.
4. **Given** a non-admin caller, **When** user creation, arbitrary update, or delete is attempted, **Then** access is denied with a documented authorization error.

---

### User Story 3 - User Self-Service Profile Access (Priority: P2)

As a standard user, I can view only my own details and update only my own details so that personal account data stays manageable without exposing other users.

**Why this priority**: This is core role behavior requested for the user role and prevents over-privileged data access.

**Independent Validation**: Validate by signing in as a user and confirming that self-view and self-update succeed, while access to other users' details is denied.

**Acceptance Scenarios**:

1. **Given** an authenticated standard user, **When** the user requests own profile details, **Then** only that user's record is returned.
2. **Given** an authenticated standard user, **When** the user updates own name and position, **Then** the update succeeds and persisted data is returned.
3. **Given** an authenticated standard user, **When** the user attempts to view or update another user's data, **Then** access is denied.

---

### User Story 4 - Agent Read-Only User Visibility (Priority: P2)

As an agent, I can view all user records or one user record, but I cannot create, update, or delete users, so that agents can inspect data without modifying it.

**Why this priority**: This role-specific visibility is explicitly requested and must be enforced distinctly from admin and user permissions.

**Independent Validation**: Validate by signing in as an agent and confirming list/detail read endpoints succeed while create/update/delete operations are denied.

**Acceptance Scenarios**:

1. **Given** an authenticated agent, **When** the agent requests all users, **Then** the API returns the user list with allowed fields and status, including soft-deleted users.
2. **Given** an authenticated agent, **When** the agent requests a specific user by identifier, **Then** the API returns that user if it exists.
3. **Given** an authenticated agent, **When** the agent attempts create, update, or delete operations, **Then** access is denied.

### Edge Cases

- What happens when admin attempts to create a user with an existing username?
- How does the system handle sign-in attempts for deleted users?
- How does the system respond when role lookup endpoint is called without a token?
- What happens when a user attempts to update restricted fields that are admin-controlled?
- How does the system behave if role seed data (user/admin/agent) is partially missing?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a role table containing exactly three initial roles: user, admin, and agent.
- **FR-002**: System MUST create a default admin account during initial setup with username "Root Admin" and password "123@Admin".
- **FR-003**: System MUST allow only admin role users to create new user accounts.
- **FR-004**: System MUST allow every created user to sign in with valid credentials.
- **FR-005**: System MUST return an authentication token upon successful sign-in.
- **FR-006**: System MUST provide a token-protected endpoint that returns the role of the authenticated caller.
- **FR-007**: System MUST allow admin role users to view all users and view any individual user details, including soft-deleted users.
- **FR-008**: System MUST allow admin role users to update any user's details.
- **FR-009**: System MUST allow admin role users to soft-delete any user record.
- **FR-010**: System MUST allow user role users to view only their own details.
- **FR-011**: System MUST allow user role users to update only their own name and position.
- **FR-012**: System MUST prevent user role users from viewing, updating, or deleting other users.
- **FR-013**: System MUST allow agent role users to view all user details and view individual user details, including soft-deleted users.
- **FR-014**: System MUST prevent agent role users from creating, updating, or deleting user records.
- **FR-015**: User data model MUST include at minimum: name, username, created date, position, and role.
- **FR-016**: System MUST store persistent data in PostgreSQL configured via environment values for host, port, database, user, and password.
- **FR-017**: System MUST use the following default connection parameters unless explicitly overridden by environment configuration: POSTGRES_HOST=localhost, POSTGRES_PORT=5432, POSTGRES_DATABASE=root_memory, POSTGRES_USER=user, POSTGRES_PASSWORD=123456.
- **FR-018**: Every endpoint MUST expose complete request and response schemas in Swagger, including success and error payloads.
- **FR-019**: Every endpoint MUST validate input and return standardized validation errors for invalid requests.
- **FR-020**: Every endpoint MUST return standardized authorization errors when role permissions are violated.
- **FR-021**: System MUST target .NET 10 and maintain Clean Architecture boundaries.
- **FR-022**: System MUST use MediatR for command/query handling in the Application layer.
- **FR-023**: System MUST use PostgreSQL through EF Core with migration-based schema changes.
- **FR-024**: System MUST emit structured logs at crucial processing steps (authentication, authorization decisions, data changes, and failures).
- **FR-025**: System MUST NOT include unit, integration, end-to-end, contract, or any other automated tests unless constitution is amended.
- **FR-026**: The default admin credentials MUST remain valid without forced password change on first login.
- **FR-027**: Soft-deleted users MUST be blocked from successful sign-in.
- **FR-028**: Admin and agent read endpoints MUST include user status and return soft-deleted users with an explicit inactive/deleted status flag.
- **FR-029**: Username, role, and created date MUST be immutable for user role self-update operations.
- **FR-030**: Authentication tokens MUST not expire automatically.

### Key Entities *(include if feature involves data)*

- **Role**: Authorization category assigned to users; required attributes include role name; seeded values are user, admin, and agent.
- **User**: Account record for authenticated access and profile data; required attributes include identifier, name, username, created date, position, role, and credential data.
- **Auth Session Token**: Returned authentication artifact representing authenticated identity and role claims used for protected endpoint access.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of documented endpoints are executable from Swagger with complete input and output schema visibility.
- **SC-002**: 100% of successful sign-in requests with valid credentials return a token and allow role lookup in a single user flow.
- **SC-003**: 100% of unauthorized role-restricted operations return documented authorization error responses instead of successful responses.
- **SC-004**: Admin users can complete create, update, and delete operations for any user in under 2 minutes via Swagger for a standard user lifecycle flow.
- **SC-005**: Standard users can access and update only their own profile while being blocked from other profiles in all attempted cross-user access cases.
- **SC-006**: Agent users can retrieve both user list and individual user details while receiving authorization errors for all data-modifying operations.

## Assumptions

- The provided default admin credentials are accepted as an initial bootstrap requirement and can be changed later through a separate feature.
- Username uniqueness is enforced to prevent duplicate identities.
- Token revocation behavior is out of scope for this feature and will be defined in a separate security-focused feature.
- The feature scope includes API behavior, data persistence, authorization rules, validation, and Swagger contract completeness.
- Automated testing remains out of scope per constitution and user instruction.
