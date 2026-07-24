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
- Get-list query handler returns `Result<PagedResult<<EntityName>Dto>>` (cursor pagination). Do not use `Result<List<...>>` for list queries.
- Update and delete command handlers return `Result`.
- Validation/not-found paths should return domain error details instead of primitive failure flags.

## Cursor Pagination Rules (Mandatory)
List queries use cursor-based (keyset) pagination, not offset/skip paging.

- The get-list handler must inherit `BasePagedQueryHandler<<FeatureFolderName>FilterDto, <EntityName>Entity>` from `JardiTips.Application/Features/Base/BasePagedQueryHandler.cs`.
- Call the inherited `BaseHandle(query.Filters, Map, ct)` to produce the paged result instead of querying the repository directly.
- The handler returns `Result<PagedResult<<EntityName>Dto>>`; assign the `BaseHandle` output directly to the result (implicit `Result` conversion).
- Provide a private static `Map(<EntityName>Entity entity)` method that projects the entity to `<EntityName>Dto`.
- Apply entity-specific filtering by overriding `protected override IQueryable<<EntityName>Entity> ModifyQuery(IQueryable<<EntityName>Entity> query, <FeatureFolderName>FilterDto request)`; do not re-implement cursor/limit logic (the base class owns cursor decoding, limit + 1 look-ahead, and `PageContext` encoding).
- The list query record wraps the filter DTO, for example `public record Get<FeatureFolderName>Query(<FeatureFolderName>FilterDto Filters);`.

Reference pattern:
- `JardiTips.Application/Features/Categories/GetCategoriesQueryHandler.cs`
- `JardiTips.Application/Features/Base/BasePagedQueryHandler.cs`
- `JardiTips.Application/Base/PagedRequestDto.cs`, `PagedResult.cs`, `PagedCursor.cs`

## Repository Usage in Handlers (Mandatory)
Use repository access through Unit of Work inside every handler.

Required pattern:
- `var repository = unitOfWork.Repository<<EntityName>Entity>();`

Example from Categories:
- `var repository = unitOfWork.Repository<CategoryEntity>();`

Usage expectations by handler type:
- Create: use repository `AddAsync(...)` and then `SaveChangesAsync(...)`.
- Delete: load by id, validate existence, then `Remove(...)` and `SaveChangesAsync(...)`.
- Get list: inherit `BasePagedQueryHandler`, call `BaseHandle(...)` with a `Map` projection, and override `ModifyQuery` for filtering; do not query the repository directly for cursor paging.
- Get by id: load via repository, validate existence, map to DTO.
- Update: load by id, mutate fields, then `SaveChangesAsync(...)`.

## Validation Rules (Mandatory)
Validate inside the handler using guard clauses. Do not add a separate validation framework or abstraction (neither third-party like FluentValidation nor a homegrown `Validator<T>`) unless rules become substantial and repetitive.

- Perform structural/input validation with DataAnnotations on the input DTOs (`[property: Required, StringLength(...)]`); keep this on the DTO, not in the handler.
- Perform business-rule and existence validation with guard clauses at the top of `HandleAsync`, before mutating state or saving.
- Return failures as domain error details, never primitive flags or exceptions:
  - `return new ErrorDetail("<kebab-case-code>", "<human readable message>", ErrorType.<Type>);`
  - Rely on the implicit `ErrorDetail` -> `Result`/`Result<T>` conversion; do not construct `Result` manually for failures.
- Error type selection:
  - `ErrorType.NotFound` when an entity cannot be loaded (e.g., wrong id or not owned by the current user).
  - `ErrorType.ValidationError` for business-rule violations (e.g., disallowed enum value or state). Maps to an RFC-7807 `ValidationProblem`.
  - `ErrorType.EntityAlreadyExists` for uniqueness conflicts.
  - `ErrorType.Unauthorized` for authentication/authorization failures.
- Ordering: run cheap in-memory rule checks first, then existence checks that hit the database, then persistence.
- Keep error `Code` values kebab-case, stable, and unique per rule so clients can branch on them.
- Structure the guard clauses to grow: for a single rule, keep it inline at the top of `HandleAsync`; when a handler has (or is expected to have) multiple rules, extract them into a `private static Result Validate(<Command> command)` method that returns `Result.Success()` when all rules pass.
- When a handler returns `Result<T>` and `Validate` returns the non-generic `Result`, short-circuit with `if (validationResult.IsFailure) return validationResult.Error!;` to leverage the implicit `ErrorDetail` -> `Result<T>` conversion.

Reference examples:
- `JardiTips.Application/Features/Categories/CreateCategoryCommandHandler.cs` (extracted `Validate` method with a `System`-type business-rule guard).

## Procedure
1. Validate naming inputs and infer paths.
2. Check for existing target handler files.
3. Stop if all 5 handlers already exist.
4. Ensure feature folder exists; create it if missing.
5. Ensure `Models` folder and DTOs exist if needed by handler contracts, and apply the mandatory DTO validation attributes to input DTOs.
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
- Filter DTO for list queries; it must extend `PagedRequestDto` so cursor paging inputs (`PageContext`, `Limit`) are inherited.

If DTOs already exist, reuse them.
If missing, create all required DTOs for a complete handler set under:
- `JardiTips.Application/Features/<FeatureFolderName>/Models/`

Required DTO coverage for full feature generation:
- `Create<EntityName>Dto`
- `Update<EntityName>Dto`
- `<EntityName>Dto`
- `<FeatureFolderName>FilterDto` (filter DTO that extends `PagedRequestDto`)

## DTO Validation Rules (Mandatory)
Input DTOs must declare validation using `System.ComponentModel.DataAnnotations` so model validation is enforced at the endpoint boundary.

Apply validation to input DTOs:
- `Create<EntityName>Dto`
- `Update<EntityName>Dto`

Attribute target rule (critical for records):
- On positional records, annotate each parameter with the `property` target so attributes bind to the generated properties, not the constructor parameters.
- Correct: `[property: Required, StringLength(250)] string Name`
- Incorrect: `[Required][StringLength(250)] string Name` (attributes land on the constructor parameter and are ignored by validation)

Attribute selection guidance:
- Use `[Required]` for mandatory fields.
- Use `[StringLength(max)]` for bounded text; keep the max length aligned with the entity/EF configuration.
- Use `[EnumDataType(typeof(<EnumType>))]` for enum inputs.
- Use `[Range(...)]`, `[EmailAddress]`, `[Url]`, and similar attributes where the field semantics require them.

Non-input DTOs:
- Output DTOs (`<EntityName>Dto`) do not require input validation attributes.
- Filter DTOs (`<FeatureFolderName>FilterDto`) extend `PagedRequestDto` and inherit its paging validation (`Limit` range). Add validation attributes only for entity-specific filter fields, keeping `[property: ...]` targeting on positional records.

Reference DTOs:
- `JardiTips.Application/Features/Categories/Models/CreateCategoryDto.cs`
- `JardiTips.Application/Features/Categories/Models/UpdateCategoryDto.cs`

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
- Handler return contracts follow the Result Pattern (`Result`/`Result<T>`), and the get-list handler returns `Result<PagedResult<<EntityName>Dto>>`.
- Get-list handler inherits `BasePagedQueryHandler` and uses `BaseHandle` for cursor pagination.
- Filter DTO extends `PagedRequestDto`.
- DTO usage is consistent and compiles.
- All required DTOs for create, update, item response, and list filter are present.
- Input DTOs (`Create<EntityName>Dto`, `Update<EntityName>Dto`) declare DataAnnotations validation with `[property: ...]` targeting on positional records.
- Business-rule and existence validation is implemented as handler guard clauses returning `ErrorDetail` with the correct `ErrorType`; no separate validation framework/abstraction was introduced.
- No unrelated files changed.

## Output Contract
Final response must include:
- Created files list.
- Reused DTOs and newly created DTOs.
- Which handlers were skipped due to already existing files.
- Build status.

## Suggested Invocation Prompt
`/command-query-handler-creator Create handlers for Tip (folder Tips) using TipEntity and DTOs CreateTipDto, UpdateTipDto, TipDto, TipsFilterDto.`
