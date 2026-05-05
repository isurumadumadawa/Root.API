# Data Model: User Role Management API

## Entity: Role

- Purpose: Defines authorization category assigned to users.
- Table: `roles`
- Fields:
  - `id` (uuid, PK)
  - `name` (varchar(20), unique, required): allowed values `user`, `admin`, `agent`
  - `created_at_utc` (timestamp with time zone, required)
- Validation rules:
  - `name` must be lowercase and one of the three allowed values.
  - Seeded role set must remain exactly three entries at initialization.
- Relationships:
  - One-to-many with `User` (`Role` -> `Users`).

## Entity: User

- Purpose: Authenticated account and profile record.
- Table: `users`
- Fields:
  - `id` (uuid, PK)
  - `name` (varchar(120), required)
  - `username` (varchar(120), unique, required)
  - `password_hash` (text, required)
  - `position` (varchar(120), nullable)
  - `role_id` (uuid, FK -> `roles.id`, required)
  - `created_at_utc` (timestamp with time zone, required, immutable)
  - `is_deleted` (boolean, required, default false)
  - `deleted_at_utc` (timestamp with time zone, nullable)
  - `updated_at_utc` (timestamp with time zone, nullable)
- Validation rules:
  - `username` must be unique (case-insensitive normalization recommended).
  - `name` required for create and self/admin update.
  - `position` optional but length-limited.
  - Self-update by `user` role can change only `name` and `position`.
  - `username`, `role_id`, and `created_at_utc` are immutable in self-update.
- Relationships:
  - Many-to-one with `Role`.
- State transitions:
  - `Active` -> `Deleted` via admin soft delete (`is_deleted = true`, `deleted_at_utc = now`).
  - Deleted users remain queryable by admin/agent endpoints.
  - Deleted users are blocked from sign-in.

## Entity: AuthToken (API contract artifact)

- Purpose: Returned credential for authenticated requests.
- Storage model: Not persisted as first-class DB table in this feature.
- Payload fields:
  - `token` (string)
  - `tokenType` (string, expected `Bearer`)
  - `issuedAtUtc` (timestamp)
  - `expiresAtUtc` (nullable, remains null because auto-expiry is disabled)
- Claims requirements:
  - Subject claim (`sub`) -> user id
  - Username claim
  - Role claim (`role`) for authorization and role endpoint response

## Derived Read Models

- UserListItem:
  - `id`, `name`, `username`, `position`, `role`, `createdDate`, `status`
- UserDetail:
  - `id`, `name`, `username`, `position`, `role`, `createdDate`, `status`
- Status mapping:
  - `Active` when `is_deleted = false`
  - `Deleted` when `is_deleted = true`

## Indexing and Constraints

- Unique index on `users.username`.
- Unique index on `roles.name`.
- Foreign key constraint on `users.role_id`.
- Optional filtered index on `users.is_deleted` to optimize active sign-in checks.

## Migration Notes

- Initial migration creates `roles` and `users` tables.
- Seed migration inserts fixed roles and default admin account.
- Subsequent migrations must preserve compatibility with immutable field and soft-delete semantics.
