# Contributing to KustoFramework

Thank you for taking the time to contribute! The following guidelines will help you get started.

## Getting Started

1. **Fork** the repository and clone your fork locally.
2. Create a new branch from `main` for your change:
   ```bash
   git checkout -b feat/my-feature
   ```
3. Make your changes, add or update tests, then verify everything passes:
   ```bash
   dotnet test
   ```
4. Push your branch and open a **Pull Request** against `main`.

## Development Setup

Requirements: **.NET 10 SDK** or later.

```bash
# Restore and build
dotnet build

# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Contribution Types

| Type | Branch prefix | Notes |
|------|--------------|-------|
| Bug fix | `fix/` | Include a failing test that reproduces the bug |
| New feature | `feat/` | Open an issue first to discuss the design |
| Documentation | `docs/` | Corrections and improvements welcome |
| Refactoring | `refactor/` | No behavior changes — must be covered by existing tests |

## Coding Guidelines

- Follow the existing code style (C# 13, nullable enabled, file-scoped namespaces).
- Keep public API surface minimal and consistent with existing operators.
- Every new operator or `Kql.*` function must have at least one corresponding unit test.
- Do not introduce external dependencies without prior discussion.

## Adding a New Operator

1. Add the rendering logic to `KqlExpressionVisitor.cs` or create a new `KqlClause` subclass in `src/KustoFramework/Query/`.
2. Expose the operator as an extension method in `src/KustoFramework/Extensions/KqlQueryExtensions.cs`.
3. Add tests in `tests/KustoFramework.Tests/`.
4. Document the operator in `docs/operators-reference.md`.

## Reporting Issues

Use the [GitHub issue tracker](../../issues). Please include:
- The KQL you expected to generate.
- The KQL that was actually generated (or the exception thrown).
- A minimal C# snippet to reproduce the problem.

## Code of Conduct

This project follows the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating, you agree to uphold it.

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).
