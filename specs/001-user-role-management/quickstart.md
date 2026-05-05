# Quickstart: User Role Management API

## 1. Prerequisites

- .NET SDK 10
- PostgreSQL 14+ running locally
- Environment variables (optional, defaults shown):

```powershell
$env:POSTGRES_HOST="localhost"
$env:POSTGRES_PORT="5432"
$env:POSTGRES_DATABASE="root_memory"
$env:POSTGRES_USER="user"
$env:POSTGRES_PASSWORD="123456"
```

## 2. Configure connection and run migrations

1. Ensure the API resolves PostgreSQL values from environment with spec defaults.
2. Add and apply EF Core migrations from Infrastructure persistence project context.
3. Verify seed data exists:
   - Roles: `user`, `admin`, `agent`
   - Default admin username: `Root Admin`

## 3. Run API

```powershell
# From repo root (C:\Project\Root\Root.API)
cd Root.API
dotnet restore
dotnet run
```

Swagger UI will be available at: `https://localhost:{port}/swagger`
(Port is printed in the console. Default dev port is 7000-7999.)

> **Note**: On first run, `MigrateAsync()` and `DatabaseSeeder.SeedAsync()` execute automatically.
> No manual migration step is needed. Ensure PostgreSQL is running before starting.

## 4. Manual validation flow (Swagger)

1. Sign in as `Root Admin` with password `123@Admin`.
2. Capture Bearer token from sign-in response.
3. Call role endpoint with token and verify role=`admin`.
4. Create a standard user and an agent user (admin-only action).
5. Sign in as each created user and validate role endpoint result.
6. As admin, update and soft-delete a user.
7. Confirm deleted user cannot sign in.
8. Confirm admin/agent list/detail endpoints include deleted users with status=`Deleted`.
9. Confirm standard user can view self and update only `name`/`position`.
10. Confirm attempts to modify `username`, `role`, `createdDate`, or other users fail with documented authorization/validation errors.

## 5. Logging and error verification

- Confirm structured logs exist for:
  - Authentication attempts and outcomes
  - Authorization decisions (allowed/denied)
  - Data modifications (create/update/soft-delete)
  - Unhandled exceptions mapped to standardized error payloads
- Confirm Swagger defines schemas for:
  - Success responses per endpoint
  - Validation errors
  - Unauthorized/forbidden errors
  - Not found/conflict errors where applicable

## 6. Explicit out-of-scope check

- Do not add unit, integration, end-to-end, contract, or other automated tests for this feature unless constitution is amended.
