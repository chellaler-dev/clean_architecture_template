# Clean Architecture Template (C# / .NET 8)

[![build](https://github.com/chellaler-dev/clean_architecture_template/actions/workflows/dotnet.yml/badge.svg)](https://github.com/chellaler-dev/clean_architecture_template/actions)
[![license](https://img.shields.io/github/license/chellaler-dev/clean_architecture_template)](LICENSE)

A minimal, opinionated template for building Web APIs with Clean Architecture using C# and .NET 8.

This repository demonstrates a practical project layout and patterns for maintainable server-side applications:

- Separate layers for Application, Domain, Infrastructure and WebApi.
- MediatR-based command/query handling, validation and pipeline behaviors (Caching and Logging).
- JWT authentication and permission-based authorization.
- EF Core-ready database layer and repository abstractions.
- Shared kernel utilities (Result/Error, Entities, DateTime provider, domain events).

## Why this repository is useful

Use this template to start new projects that benefit from:

- Clear separation of concerns (Domain, Application, Infrastructure, Presentation).
- Testable business logic isolated from framework concerns.
- Ready-to-use behaviors (logging, validation, caching) and common abstractions.
- Examples of user-related flows (commands/queries under `src/Application/Users`).

## Repository layout

Top-level folders you will use most:

- `src/WebApi` — Minimal API entrypoint and HTTP configuration.
- `src/Application` — Application services, MediatR handlers, DTOs and pipeline behaviors.
- `src/Domain` — Domain entities, enums, domain events and repository interfaces.
- `src/Infrastructure` — Implementations for persistence, authentication, authorization, caching, and DI wiring.
- `src/SharedKernel` — Lightweight shared primitives (Result, Error, Entity helpers).

See the full tree in the repository for additional folders and implementation details.

## Getting started

Prerequisites

- .NET 8 SDK (dotnet 8.x) installed. Verify with:

```bash
dotnet --version
```

Clone and build

```bash
git clone https://github.com/chellaler-dev/clean_architecture_template.git
cd clean_architecture_template
dotnet build
```

Run the Web API

```bash
dotnet run --project src/WebApi/WebApi.csproj
```

By default the minimal API will read configuration from `appsettings.json` / `appsettings.Development.json`. JWT and other infra options are configured under `src/Infrastructure` (see `Authentication/JwtOptions.cs` and provider code).

Development (VS Code)

- A VS Code task is configured for building the WebApi (`process: build WebApi`). Open the Command Palette and run the task, or run the command in the workspace:

```bash
dotnet build src/WebApi/WebApi.csproj
```

Quick notes

- The project targets `net8.0` and uses `MediatR`, `Swashbuckle` (OpenAPI), JWT authentication and EF Core design-time packages.
- Application layer contains example user flows: `CreateUser`, `GetUserByEmail`, `GetUserById`, and `Login`.

## Configuration

- `src/WebApi/appsettings.json` and `appsettings.Development.json` hold runtime configuration.
- JWT settings and secret management live under `src/Infrastructure/Authentication` — replace secrets with secure store in production.

## Working with the code

- Add new domain logic under `src/Domain` and expose operations through `src/Application` handlers.
- Register infrastructure services in `src/Infrastructure/DependencyInjection.cs` when adding implementations (e.g., EF Core DbContext, caches, external services).
- Keep controllers/endpoints thin — orchestrate work through MediatR commands and queries.

## Tests

This template does not include a test project by default. We recommend adding test projects for `Application` and `Domain` layers (unit tests) and a small integration test suite for the Web API.

## Contributing and support

- Found a bug, need a feature, or want to discuss the design? Open an issue in this repository.
- Contribution guidelines: see `CONTRIBUTING.md` or `.github/CONTRIBUTING.md` (if present). If not present, please open an issue to discuss contributions before sending large PRs.

## Maintainers

- Maintained by the repository owner: `chellaler-dev`.
- Prefer issues and pull requests for communication. For quick questions, add a short note in an issue.

## License

This project includes a `LICENSE` file at the repository root. See `LICENSE` for license details.

