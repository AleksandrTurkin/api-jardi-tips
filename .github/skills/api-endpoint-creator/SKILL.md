---
name: api-endpoint-creator
description: 'Create Web API endpoint classes for an entity in JardiTips. Implements IEndpoint, registers CQRS handlers in DI, and maps routes using EndpointMapExtensions following CategoryEndpoint pattern.'
argument-hint: 'Entity name, plural route, feature namespace, command/query and DTO types'
user-invocable: true
---

# API Endpoint Creator

## Purpose
Create a new endpoint class for a specified entity in the Web API layer, following the existing endpoint architecture.

This skill creates endpoint classes that:
- Implement `IEndpoint`.
- Register command/query handler services.
- Map HTTP routes through `EndpointMapExtensions`.
- Use `Result` and `Result<T>` handler contracts compatible with HTTP result conversion.
- Use `Result<PagedResult<TDto>>` for list (filter) queries with cursor-based pagination.

## Scope
Responsible area:
- `JardiTips.WebApi/JardiTips.WebApi/Endpoints/`

Primary references:
- `JardiTips.WebApi/JardiTips.WebApi/Endpoints/Base/IEndpoint.cs`
- `JardiTips.WebApi/JardiTips.WebApi/Extensions/EndpointMapExtensions.cs`
- `JardiTips.WebApi/JardiTips.WebApi/Endpoints/CategoryEndpoint.cs`

## Inputs
The user provides:
- `EntityName` (singular), for example `Category`.
- `EntityRoute` (plural route segment), for example `categories`.
- Application feature namespace, for example `JardiTips.Application.Features.Categories`.
- Command/query types and DTO types.

Input rule:
- `EntityName` should be singular.
- If a plural entity name is provided, convert it to singular automatically when the conversion is clear.
- If plural-to-singular conversion is ambiguous (irregular noun), ask for confirmation before generating endpoint code.

## Use When
Use this skill when the request is to add or scaffold a new entity endpoint class in Web API.

Do not use this skill for:
- Creating Application handlers.
- Creating Domain entities or EF migrations.
- Changing endpoint wrapper extension behavior globally.

## Endpoint Contract (Mandatory)
All generated endpoints must:
- Implement `IEndpoint`.
- Contain `Register(IServiceCollection services)`.
- Contain `Map(IEndpointRouteBuilder builder)`.
- Register each handler using `AddScoped` with the matching command/query interface.
- Map routes only via methods in `EndpointMapExtensions`.

## Procedure
1. Validate required types and route inputs.
2. Check if endpoint file already exists in `Endpoints/`.
3. Stop if the endpoint already exists and no changes are requested.
4. Create endpoint class file named `<EntityName>Endpoint.cs`.
5. Add required `using` statements:
- `JardiTips.Application.Base`
- Feature namespace and feature `.Models` namespace
- `JardiTips.Domain.Common`
- `JardiTips.WebApi.Endpoints.Base`
- `JardiTips.WebApi.Extensions`
6. Implement `Register` method:
- Register create command handler (`ICommandHandler<..., Result<Guid>>`).
- Register get-by-id query handler (`IQueryHandler<..., Result<Dto>>`).
- Register get-list query handler (`IQueryHandler<..., Result<PagedResult<Dto>>>`).
- Register update command handler (`ICommandHandler<..., Result>`).
- Register delete command handler (`ICommandHandler<..., Result>`).
7. Implement `Map` method:
- Create group with `builder.MapGroup("/<EntityRoute>").WithTags("<EntityName>")`.
- Map create via `MapPostCommand<TRequest, TDto>`.
- Map get-by-id via `MapGetByIdQuery<TRequest, TResponse, TKey>`.
- Map list via `MapGetFilterQuery<TRequest, PagedResult<TDto>, TFilter>` where `TFilter` extends `PagedRequestDto`.
- Map update via `MapPutCommand<TRequest, TDto, TKey>`.
- Map delete via `MapDeleteCommand<TRequest, TKey>`.
8. Keep behavior consistent with `CategoryEndpoint` structure and formatting.
9. Build solution and fix only issues introduced by this change.
10. Summarize registrations, routes, and mapped request types.

## Handler Registration Rules
Use explicit registration pattern:
- `services.AddScoped<ICommandHandler<TCommand, Result<Guid>>, TCreateHandler>();`
- `services.AddScoped<IQueryHandler<TQueryById, Result<TDto>>, TGetByIdHandler>();`
- `services.AddScoped<IQueryHandler<TListQuery, Result<PagedResult<TDto>>>, TGetListHandler>();`
- `services.AddScoped<ICommandHandler<TUpdateCommand, Result>, TUpdateHandler>();`
- `services.AddScoped<ICommandHandler<TDeleteCommand, Result>, TDeleteHandler>();`

## Route Mapping Rules
Use extension mapping methods from `EndpointMapExtensions` only:
- `MapPostCommand<TRequest, TDto>`
- `MapGetByIdQuery<TRequest, TResponse, TKey>`
- `MapGetFilterQuery<TRequest, PagedResult<TDto>, TFilter>` (list query returns a cursor-paged `PagedResult<TDto>`; `TFilter : PagedRequestDto`)
- `MapPutCommand<TRequest, TDto, TKey>`
- `MapDeleteCommand<TRequest, TKey>`

Cursor pagination note:
- The list route response type is `PagedResult<TDto>`, which carries a `PageContext` cursor and `Data`. Reference `CategoryEndpoint.Map` where list is mapped as `MapGetFilterQuery<GetCategoriesQuery, PagedResult<CategoryDto>, CategoriesFilterDto>`.

Endpoint group naming rule (mandatory):
- Use `var group = builder.MapGroup("/<plural-entity>").WithTags("<singular-entity>");`.
- Example: `var group = builder.MapGroup("/categories").WithTags("Category");`.
- `categories` is plural entity route segment.
- `Category` is singular entity name used as API tag.

Canonical route patterns:
- Create: `""`
- Get by id: `"{id:guid}"`
- Get list: `""`
- Update: `"{id:guid}"`
- Delete: `"{id:guid}"`

## Decision Branches
- If endpoint file exists: stop and ask whether to overwrite or patch.
- If one or more handler types are missing in Application layer: stop and ask user to generate handlers first using `/command-query-handler-creator`.
- If DTO types are missing for mapping signatures: stop and ask user to generate DTOs and handlers first using `/command-query-handler-creator`.
- If route pluralization is unclear: ask for explicit route segment.

## Completion Checks
- Endpoint class implements `IEndpoint`.
- `Register` includes all required AddScoped entries.
- `Map` includes all five extension-based route mappings.
- Route base group and tag align with entity naming.
- No direct inline minimal-api handler duplication outside extension helpers.

## Output Contract
Final response must include:
- Created or updated endpoint file path.
- Registered service bindings summary.
- Route mapping summary.
- Build status.

## Suggested Invocation Prompt
`/api-endpoint-creator Create Tip endpoint with route tips using JardiTips.Application.Features.Tips command/query handlers and DTOs.`
