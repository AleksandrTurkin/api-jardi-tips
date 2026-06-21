---
name: command-query-handler-creator
description: 'Create CQRS command and query handlers for a new entity in JardiTips Application Features. Generates 5 handlers in a plural entity folder, uses DTOs when needed, and enforces ICommandHandler and IQueryHandler interfaces.'
argument-hint: 'Entity name, plural feature folder name, and required DTOs'
user-invocable: true
---

# Command Query Handler Creator

## Purpose
Create a complete Application CQRS handler set for one entity in the JardiTips codebase.

This skill creates the standard 5 handlers under the entity feature folder and follows the Categories pattern.

## Inputs
The user provides:
- EntityName in singular PascalCase, for example User.
- FeatureFolderName in plural PascalCase, for example Users.
- Entity class name in Domain, for example UserEntity.
- DTO availability details (existing DTOs or request to create missing DTOs).

Input rule:
- `EntityName` should be singular.
- If a plural entity name is provided, convert it to singular automatically when the conversion is clear.
- If plural-to-singular conversion is ambiguous (irregular noun), ask for confirmation before creating files.

## Use When
Use this skill when the request is to scaffold CQRS handlers for a new entity feature in:
- `JardiTips.Application/Features/<FeatureFolderName>/`

Do not use this skill for:
- Endpoint creation in Web API.
- Database entity and migration creation.
- Refactoring existing handlers with behavioral changes outside scaffold scope.

## Reference Pattern
Mirror structure and style from:
- `JardiTips.Application/Features/Categories/CreateCategoryCommandHandler.cs`
- `JardiTips.Application/Features/Categories/DeleteCategoryCommandHandler.cs`
- `JardiTips.Application/Features/Categories/GetCategoriesQueryHandler.cs`
- `JardiTips.Application/Features/Categories/GetCategoryByIdQueryHandler.cs`
- `JardiTips.Application/Features/Categories/UpdateCategoryCommandHandler.cs`
- `JardiTips.Application/Features/Categories/Models/`

## Required Handler Set
Create these 5 files in:
- `JardiTips.Application/Features/<FeatureFolderName>/`

1. `Create<EntityName>CommandHandler.cs`
2. `Delete<EntityName>CommandHandler.cs`
3. `Get<FeatureFolderName>QueryHandler.cs` (multiple items)
4. `Get<EntityName>ByIdQueryHandler.cs`
5. `Update<EntityName>CommandHandler.cs`

Example for `User` and `Users`:
- `CreateUserCommandHandler`
- `DeleteUserCommandHandler`
- `GetUsersQueryHandler`
- `GetUserByIdQueryHandler`
- `UpdateUserCommandHandler`

## Interface Rules (Mandatory)
- All query handlers must implement `IQueryHandler<TQuery, TResult>` from `JardiTips.Application/Base/IQueryHandler.cs`.
- All command handlers must implement `ICommandHandler<TCommand, TResult>` from `JardiTips.Application/Base/ICommandHandler.cs`.

## Result Pattern Rules (Mandatory)
- All handler result types must use `JardiTips.Domain.Common.Result` wrappers.
- Create command handler returns `Result<Guid>`.
- Get-by-id query handler returns `Result<<EntityName>Dto>`.
- Get-list query handler returns `Result<List<<EntityName>Dto>>`.
- Update and delete command handlers return `Result`.
- Validation/not-found paths should return domain error details instead of primitive failure flags.

## Repository Usage in Handlers (Mandatory)
Use repository access through Unit of Work inside every handler.

Required pattern:
- `var repository = unitOfWork.Repository<<EntityName>Entity>();`

Example from Categories:
- `var repository = unitOfWork.Repository<CategoryEntity>();`

Usage expectations by handler type:
- Create: use repository `AddAsync(...)` and then `SaveChangesAsync(...)`.
- Delete: load by id, validate existence, then `Remove(...)` and `SaveChangesAsync(...)`.
- Get list: query via repository and map entities to DTO collection.
- Get by id: load via repository, validate existence, map to DTO.
- Update: load by id, mutate fields, then `SaveChangesAsync(...)`.

## Procedure
1. Validate naming inputs and infer paths.
2. Check for existing target handler files.
3. Stop if all 5 handlers already exist.
4. Ensure feature folder exists; create it if missing.
5. Ensure `Models` folder and DTOs exist if needed by handler contracts.
6. Create command/query record types and handler classes in each file.
7. Inject `IUnitOfWork` and use repository access against `<EntityName>Entity`.
8. Add mapping methods between entity and DTO where needed, and return mapped values through `Result` wrappers.
9. Build the solution and fix only issues introduced by this run.
10. Summarize created files, reused DTOs, and new DTOs.

## DTO Guidance
DTOs should follow the pattern in:
- `JardiTips.Application/Features/Categories/Models/CategoryDto.cs`
- `JardiTips.Application/Features/Categories/Models/CreateCategoryDto.cs`
- `JardiTips.Application/Features/Categories/Models/UpdateCategoryDto.cs`
- `JardiTips.Application/Features/Categories/Models/CategoriesFilterDto.cs`

Use DTOs when needed:
- Create command input DTO for create operations.
- Update command input DTO for update operations.
- Output DTO for query results.
- Filter DTO for list queries when list filtering/paging input is required.

If DTOs already exist, reuse them.
If missing, create all required DTOs for a complete handler set under:
- `JardiTips.Application/Features/<FeatureFolderName>/Models/`

Required DTO coverage for full feature generation:
- `Create<EntityName>Dto`
- `Update<EntityName>Dto`
- `<EntityName>Dto`
- `<FeatureFolderName>FilterDto` (or equivalent filter DTO that extends `PagerRequestDto`)

## Naming and Folder Rules
- Feature folder must be plural, for example `Users`, `Tips`, `Categories`.
- Handler class names stay singular where entity-specific (`CreateUser...`, `GetUserById...`).
- List query handler uses plural folder/entity collection name (`GetUsersQueryHandler`).
- Keep namespace aligned with folder path, for example `JardiTips.Application.Features.<FeatureFolderName>` and `JardiTips.Application.Features.<FeatureFolderName>.Models`.

## Decision Branches
- If all 5 handlers already exist: stop with no changes.
- If only some handlers exist: create only missing handlers and preserve existing files.
- If DTO names are ambiguous: ask a targeted question, but still ensure all required DTOs are created or mapped before completion.
- If plural folder name is missing: derive from EntityName and confirm if irregular pluralization is possible.
- If build fails: fix only changes introduced by this run.

## Completion Checks
- Exactly 5 handlers exist for the feature.
- Query handlers implement `IQueryHandler`.
- Command handlers implement `ICommandHandler`.
- Handler return contracts follow the Result Pattern (`Result`/`Result<T>`).
- DTO usage is consistent and compiles.
- All required DTOs for create, update, item response, and list filter are present.
- No unrelated files changed.

## Output Contract
Final response must include:
- Created files list.
- Reused DTOs and newly created DTOs.
- Which handlers were skipped due to already existing files.
- Build status.

## Suggested Invocation Prompt
`/command-query-handler-creator Create handlers for Tip (folder Tips) using TipEntity and DTOs CreateTipDto, UpdateTipDto, TipDto, TipsFilterDto.`
