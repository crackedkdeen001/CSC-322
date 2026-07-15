# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
dotnet build                                  # build the whole solution
dotnet run --project GenericAlgorithm         # run the console demo (Program.cs)
dotnet test                                   # run all tests
dotnet test --filter "FullyQualifiedName~SortsAListOfIntegersInAscendingOrder"   # run a single test
dotnet test --filter "FullyQualifiedName~GenericAlgorithmTest.UnitTest1"         # run one test class
```

Targets .NET 10 (SDK 10.0.301). Tests are xUnit; `Xunit` is a global `<Using>` in the test csproj, so test files don't need `using Xunit;`.

## Structure

Two projects in `GenericAlgorithm.sln`:

- **GenericAlgorithm** — library code plus a top-level-statements `Program.cs` that acts as a scratch demo harness (`OutputType=Exe`). Algorithms live in their own files (e.g. `GenericInsertionSort.cs`) as static methods on a public class.
- **GenericAlgorithmTest** — xUnit project referencing GenericAlgorithm.

`ImplicitUsings` and `Nullable` are enabled in both projects.

## Conventions

Algorithms follow the pattern set by `GenericInsertionSort.Sort<T>`: a `public static` generic method constrained with `where T : IComparable<T>`, documented with XML doc comments that spell out an explicit **PreCondition** and **PostCondition**. Match that documentation style when adding new algorithms.

Note that `Sort<T>` sorts the caller's `List<T>` **in place** and returns the same reference — it does not copy. Tests and callers that need the original ordering must clone the list first.