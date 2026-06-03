# Copilot Instructions

## Project Overview

- Framework: .NET 10 Web API
- Architecture: Clean Architecture
- Database: PostgreSQL with EF Core
- API Design: RESTful with CQRS pattern
- Endpoints: Minimal APIs with reflection-based common wrapper (see `JardiTips.WebApi/Extensions/EndpointMapExtensions.cs`)

## How the app is wired

- Endpoints are discovered via reflection from types implementing `IEndpoint`. Follow the existing pattern instead of manual endpoint registration.
- Requests flow through endpoint classes into CQRS handlers in `JardiTips.Application`.
- Data access goes through `IUnitOfWork` and `IRepository<T>`, then EF Core in `JardiTips.Infrastructure`.

## Data and environment notes

- The API uses PostgreSQL through Npgsql and EF Core.
- Database initialization runs on startup. Check the infrastructure data extensions before changing startup behavior.
- CORS and Swagger are enabled in development. Keep production-facing behavior explicit.

## Change guidance for agents

- Prefer small edits inside the owning layer instead of cross-cutting rewrites.
- Do not move business rules into the Web API layer when they belong in Application or Domain.
- Do not bypass `IUnitOfWork` and `IRepository<T>` without a clear reason already established in the codebase.
- If you change endpoint wiring, verify the app still builds from the solution root.