---
name: apply-database-migration
description: 'Manual-only workflow that applies pending JardiTips EF Core migrations. Use only when the user explicitly invokes apply-database-migration in the current request.'
user-invocable: true
---

# Apply Database Migration

## Purpose

Apply pending EF Core migrations to the configured JardiTips database.

## Manual Invocation Gate

Run this skill only when the current user request explicitly invokes `apply-database-migration` by name or explicitly asks to apply/update the JardiTips database migration.

Do not run this skill:

- Automatically after creating or editing a migration.
- When the user asks only to generate, inspect, explain, or prepare a migration.
- Based on an earlier request or inferred intent.
- As part of another skill or workflow unless the current user message explicitly requests database application.

If the manual invocation requirement is not met, stop without running any command and explain that applying migrations requires explicit invocation.

## Procedure

1. Confirm the workspace root contains `JardiTips.Infrastructure` and `JardiTips.WebApi/JardiTips.WebApi`.
2. Show the user that the migration will be applied to the database configured by the Web API startup project.
3. Run this command from the repository root:

   `dotnet ef database update --project JardiTips.Infrastructure --startup-project JardiTips.WebApi/JardiTips.WebApi`

4. Report whether the command succeeded and include any EF Core error or warning that requires action.

## Safety Rules

- Do not add, remove, or modify migration files.
- Do not change connection strings or environments.
- Do not substitute another project or startup project.
- Do not suppress command output or failures.
- Never run `dotnet ef database drop`.
