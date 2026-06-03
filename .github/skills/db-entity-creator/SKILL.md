---
name: db-entity-creator
description: 'Create new EF Core entities and mappings for JardiTips, then add a migration without applying it. Use when asked to add a DB entity, configure table mapping, and generate migration files from field definitions. Stops immediately if entity already exists.'
argument-hint: 'Entity name + fields (name, type, required, max length, description)'
user-invocable: true
---

# DB Entity Creator

## Purpose
Create a new database entity in the JardiTips API using the established Clean Architecture and EF Core patterns.

This skill:
- Creates the domain entity class.
- Creates the EF Core entity configuration.
- Registers configuration in the DbContext flow if needed.
- Adds an EF Core migration.

This skill never runs database update commands.

## Inputs
The user provides:
- Entity name.
- Fields for the entity.
- Description for each field.
- Optional constraints: required/optional, max length, uniqueness, defaults.

## Use When
Use this skill when the request is to introduce a new persistable model and matching migration files in this repository.

Do not use this skill for:
- Updating existing entities with complex refactors across many bounded contexts.
- Runtime data fixes.
- Applying migrations to a live database.

## Project Pattern To Mirror
Use these files as the source pattern for style and conventions:
- `JardiTips.Domain/Entities/CategoryEntity.cs`
- `JardiTips.Infrastructure/Configurations/CategoryConfiguration.cs`

Conventions to preserve:
- Entity in Domain layer.
- EF configuration in Infrastructure layer.
- Mapping via `IEntityTypeConfiguration<T>`.
- Required fields and max lengths configured with Fluent API.
- Keep naming consistent with existing entities and configurations.

## Procedure
1. Validate input completeness.
2. Pre-check for existing entity and configuration.
3. Stop immediately if entity already exists.
4. Create the domain entity file.
5. Create the EF Core configuration file.
6. Wire up DbContext mapping only if explicit registration is required by existing project style.
7. Generate migration files with `dotnet ef migrations add <MigrationName>` from the correct startup/project context.
8. Do not run `dotnet ef database update`.
9. Build solution to validate compile health.
10. Summarize created files and migration name.

## Detailed Execution Rules

### 1) Validate input
Before editing files, ensure input includes:
- Entity name in singular PascalCase ending with `Entity` in code artifact names only when consistent with repo conventions.
- At least one non-key business field.
- Field list with type and field description.

If input is incomplete, ask for missing values before editing.

Naming terms used by this skill:
- `EntityName`: the only name the user provides, singular PascalCase class name, for example `ExpenseEntity`.
- `EntityBaseName`: singular class stem used for configuration and migration naming, for example `Expense`.
- `PluralTableName`: plural table name, for example `Expenses`.

Naming derivation rule:
- Derive `EntityBaseName` from `EntityName` by removing a trailing `Entity` suffix if present.
- Use singular configuration naming: `<EntityBaseName>Configuration` (for example `CategoryConfiguration`, not `CategoriesConfiguration`).

### 2) Existence checks (hard stop)
Check for existing files and symbols:
- `JardiTips.Domain/Entities/<EntityName>.cs`
- `JardiTips.Infrastructure/Configurations/<EntityBaseName>Configuration.cs`
- Existing class declarations for the target entity in Domain and Infrastructure.

If entity exists:
- Stop execution.
- Do not modify files.
- Return a clear message: entity already exists and no changes were made.

### 3) Domain entity creation
Create in:
- `JardiTips.Domain/Entities/<EntityName>.cs`

Apply these defaults unless user says otherwise:
- `Guid Id` primary key.
- `DateTime CreatedAt`.
- `DateTime UpdatedAt`.
- Additional properties from user field definitions.

Map user field descriptions into concise XML comments only when clarification is needed for non-obvious fields.

### 4) EF configuration creation
Create in:
- `JardiTips.Infrastructure/Configurations/<EntityBaseName>Configuration.cs`

Required mapping steps:
- `ToTable("<PluralTableName>")`.
- `HasKey(x => x.Id)`.
- `Property(x => x.Id).ValueGeneratedOnAdd()`.
- Per-property constraints from input (required/optional, max length, and column type/defaults only when explicitly requested or already standard in repo).

Do not add speculative indexes/relationships unless requested.

### 5) DbContext registration
Review current pattern in `EntityDbContext`:
- If `ApplyConfigurationsFromAssembly` is used, no manual registration needed.
- If explicit registrations are used, add only the new configuration.

### 6) Migration generation
Run migration add command with correct project targeting for this repo. Example shape:
- `dotnet ef migrations add Add<EntityBaseName> --project JardiTips.Infrastructure --startup-project JardiTips.WebApi/JardiTips.WebApi`

Pluralization rule:
- Do not pluralize `EntityName` or `EntityBaseName` for class/config names.
- Only pluralize `<PluralTableName>` used in `ToTable(...)`.

Accept generated files under:
- `JardiTips.Infrastructure/Migrations/`

Never run:
- `dotnet ef database update`

### 6.1) Rollback guidance
If migration creation or validation fails, use rollback in this order:
- If the last migration was just created and not applied to a database, run `dotnet ef migrations remove --project JardiTips.Infrastructure --startup-project JardiTips.WebApi/JardiTips.WebApi`.
- If migration files were partially created, remove only the new migration files from `JardiTips.Infrastructure/Migrations/` and restore compile state.
- If multiple files were edited before failure, revert only files introduced by this run and keep unrelated workspace changes untouched.

Rollback constraints:
- Never run `dotnet ef database update` during rollback.
- Never execute destructive git commands.

### 7) Validation and completion checks
Completion criteria:
- Entity class exists and compiles.
- Configuration class exists and compiles.
- Migration files created.
- No unrelated file rewrites.
- Database update command not executed.

Final response must include:
- Created/updated files.
- Migration name.
- Constraints applied per field.
- Explicit confirmation that database update was not run.

## Decision Branches
- If entity exists already: stop with no edits.
- If field constraints are ambiguous: ask targeted follow-up questions.
- If migration command fails due to tooling context: adjust `--project` and `--startup-project` and retry once.
- If retry still fails: execute rollback guidance for newly created migration artifacts and report exactly what was reverted.
- If build fails due to new code: fix only issues introduced by this change.

## Guardrails
- Keep business rules out of Web API layer.
- Keep edits minimal and localized to Domain and Infrastructure unless required.
- Follow existing naming and nullable-reference conventions in each edited file.
- Never execute destructive DB commands.

## Suggested Invocation Prompt
`/db-entity-creator Create ExpenseEntity with fields: Amount(decimal, required, "Total amount"), Currency(string, required, max 3, "ISO code"), Notes(string, optional, max 500, "Optional note")`
