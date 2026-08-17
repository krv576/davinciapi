# Coding Standards

Applies to all C# code in this repository. See also the path-scoped [.github/instructions/](../.github/instructions/) files for area-specific rules.

## Language & project settings

- .NET 8, `LangVersion` default (C# 12).
- `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>` in every project — do not suppress nullable warnings with `!` unless truly unavoidable, and never silently swallow them with `#pragma warning disable`.
- One public type per file; file name matches the type name.

## Naming

- `PascalCase` for types, methods, properties, public fields, and constants.
- `camelCase` for locals and parameters; `_camelCase` for private fields.
- Interfaces prefixed with `I` (`IPriorAuthRequestRepository`).
- Async methods suffixed with `Async` (`SubmitAsync`, `GetByIdAsync`).
- Namespaces mirror folder structure under the project root namespace (e.g. `DavinciEPA.Core.PriorAuthorization`).

## Language features & patterns

- Use `record`/`record class` for immutable DTOs and value objects; use `class` for services and entities with mutable state/identity.
- Prefer expression-bodied members for trivial one-liners; use full bodies once logic exceeds a single expression.
- Use pattern matching (`switch` expressions, `is` patterns) over chained `if/else` where it improves clarity.
- Use `async`/`await` throughout the call chain — never block on async code with `.Result`/`.Wait()`/`GetAwaiter().GetResult()`.
- Every async method that can be cancelled accepts a `CancellationToken` as its last parameter and forwards it downstream.
- Prefer dependency injection (constructor injection) over static/singleton service access.
- Favor composition over inheritance; avoid deep class hierarchies.

## Error handling

- Use exceptions only for truly exceptional/unexpected conditions (e.g. infrastructure failures), not for expected business outcomes (e.g. "coverage requirement not met") — model expected outcomes with a `Result<T>`/error type from `DavinciEPA.Shared`.
- Never swallow exceptions silently (`catch { }`); log with context (no PHI) and rethrow or translate to an appropriate response.

## Formatting

- 4-space indentation, standard `dotnet format`/`.editorconfig` conventions (add an `.editorconfig` at the repo root if one doesn't already enforce this).
- Braces on new lines (Allman style, matching default `dotnet new` templates already in this repo).
- Keep methods focused; extract a private method or new class rather than growing a method past a screen or two.

## Comments & documentation

- Write a comment only to explain *why*, not *what* — the code should already show what it does.
- Do not leave `TODO` comments in committed code (per repository policy); either finish the work or track it outside the codebase.
- XML doc comments (`///`) are optional for internal code; use them for public interfaces in `Core` where the contract isn't obvious from the signature alone.

## Linting

- Treat new compiler warnings as blocking for the change that introduced them — fix them rather than suppressing.
- Run `dotnet format` before committing if formatting drifts from the above.
