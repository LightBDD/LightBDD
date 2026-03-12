# Migration Guide: LightBDD.XUnit2 → LightBDD.XUnit3

This guide walks you through migrating an existing LightBDD project from **xUnit v2** (`LightBDD.XUnit2`) to **xUnit v3** (`LightBDD.XUnit3`). Each step is small and mechanical — the user-facing API is intentionally similar.

---

## Step 1: Update NuGet packages

Remove the old packages and add the new ones.

**Before (xUnit v2):**
```xml
<PackageReference Include="LightBDD.XUnit2" Version="..." />
<PackageReference Include="xunit" Version="2.4.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.6.3" />
```

**After (xUnit v3):**
```xml
<PackageReference Include="LightBDD.XUnit3" Version="..." />
<PackageReference Include="xunit.v3" Version="3.2.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="3.1.0" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
```

---

## Step 2: Make your test project an executable

xUnit v3 test assemblies must be executables, not class libraries. Add `<OutputType>Exe</OutputType>` and update to a supported target framework (net8.0+).

**Before:**
```xml
<TargetFrameworks>net6;net48</TargetFrameworks>
```

**After:**
```xml
<TargetFramework>net8.0</TargetFramework>
<OutputType>Exe</OutputType>
```

---

## Step 3: Update the LightBDD scope registration

This is the most important change. xUnit v3 uses `[assembly: TestPipelineStartup(...)]` instead of a custom assembly attribute.

**Before (xUnit v2):**
```csharp
using LightBDD.XUnit2;

[assembly: ConfiguredLightBddScope]

namespace MyTests
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            // your configuration here
        }
    }
}
```

**After (xUnit v3):**
```csharp
using LightBDD.XUnit3;
using Xunit.v3;

[assembly: TestPipelineStartup(typeof(ConfiguredLightBddScope))]

namespace MyTests
{
    public class ConfiguredLightBddScope : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            // your configuration here — no changes needed
        }
    }
}
```

**What changed:**
1. `[assembly: ConfiguredLightBddScope]` → `[assembly: TestPipelineStartup(typeof(ConfiguredLightBddScope))]`
2. Class must be **`public`** (was `internal`) — xUnit v3 requires this for `TestPipelineStartup`
3. Class name convention: drop the `Attribute` suffix (e.g., `ConfiguredLightBddScopeAttribute` → `ConfiguredLightBddScope`) — not required, but conventional
4. Add `using Xunit.v3;`
5. Replace `using LightBDD.XUnit2;` with `using LightBDD.XUnit3;`

---

## Step 4: Remove `ClassCollectionBehavior` and configure parallelization

`[assembly: ClassCollectionBehavior(...)]` does not exist in `LightBDD.XUnit3`. It was a LightBDD-specific workaround for an xUnit v2 limitation. In xUnit v3, parallelization is handled natively by xUnit and no LightBDD attribute is needed.

**Background:** In xUnit v2, all test classes without an explicit `[Collection("name")]` attribute were placed into a single implicit collection, and xUnit ran classes within the same collection *sequentially*. This meant that by default, your test classes did not run in parallel. LightBDD's `ClassCollectionBehavior` attribute worked around this by adding custom inter-class parallelization within that single collection.

In xUnit v3, each test class is its own collection by default, so **inter-class parallelization works out of the box** — no workaround needed.

### If you had `AllowTestParallelization = true`

**Before (xUnit v2):**
```csharp
[assembly: ClassCollectionBehavior(AllowTestParallelization = true)]
```

**After (xUnit v3):**
*(just delete the line — this is the default behavior in xUnit v3)*

xUnit v3 runs test classes in parallel by default. No configuration needed.

### If you had `AllowTestParallelization = false` (or didn't use it at all)

**Before (xUnit v2):**
```csharp
// No ClassCollectionBehavior, or:
[assembly: ClassCollectionBehavior(AllowTestParallelization = false)]
```

In xUnit v2, this meant all test classes in the default collection ran sequentially. In xUnit v3, the default is parallel, so if you **want to preserve sequential behavior**, you need to explicitly disable parallelization:

**After (xUnit v3) — to keep tests sequential:**
```csharp
[assembly: CollectionBehavior(DisableTestParallelization = true)]
```

### Summary

| XUnit2 setting | Behavior | XUnit3 equivalent |
|---|---|---|
| `AllowTestParallelization = true` | Classes run in parallel | Default — just delete the attribute |
| `AllowTestParallelization = false` or absent | Classes run sequentially | `[assembly: CollectionBehavior(DisableTestParallelization = true)]` |

> **Note:** If you used `[Collection("name")]` on specific classes to group them together, this still works in xUnit v3. Classes in the same named collection run sequentially with respect to each other, while different collections run in parallel.

> **Alternative: `xunit.json`** — Parallelization can also be controlled via xUnit v3's JSON configuration file (`xunit.json`, placed in your project root and copied to output). For example, to disable parallelization:
> ```json
> {
>   "parallelizeTestCollections": false
> }
> ```
> Or to limit the degree of parallelism:
> ```json
> {
>   "maxParallelThreads": 4
> }
> ```
> See the [xUnit v3 configuration docs](https://xunit.net/docs/configuration-files) for the full list of settings.

> **Configuration file rename:** If you have an existing `xunit.runner.json` from xUnit v2, rename it to **`xunit.json`** for xUnit v3 and update the `<CopyToOutputDirectory>` item in your `.csproj` accordingly.

---

## Step 5: Find-and-replace namespace imports

In every file that references LightBDD.XUnit2, replace the namespace:

| Find | Replace with |
|---|---|
| `using LightBDD.XUnit2;` | `using LightBDD.XUnit3;` |

That's it. All the public types (`FeatureFixture`, `ScenarioAttribute`, `IgnoreScenarioAttribute`, `IgnoreScenarioIfAttribute`, `FeatureFixtureAttribute`, `ITestOutputProvider`, `FeatureRunnerProvider`, `StepExecutionExtensions`) have the same names and the same APIs.

---

## Step 6: Remove `ITestOutputHelper` constructor parameters (if any)

In xUnit v2, the `FeatureFixture` constructor accepted an optional `ITestOutputHelper` parameter (marked `[Obsolete]`). In xUnit v3, this constructor is removed entirely because `TestContext.Current` provides test output automatically.

If any of your fixture classes pass `ITestOutputHelper` to the base constructor, remove it:

**Before:**
```csharp
public class My_feature : FeatureFixture
{
    public My_feature(ITestOutputHelper output) : base(output) { }
}
```

**After:**
```csharp
public class My_feature : FeatureFixture
{
}
```

If you were storing `ITestOutputHelper` to write custom output within steps, use the `TestOutput` property instead (it now resolves automatically via `TestContext.Current`):

**Before (xUnit v2):**
```csharp
public class My_feature : FeatureFixture
{
    private readonly ITestOutputHelper _output;

    public My_feature(ITestOutputHelper output) : base(output)
    {
        _output = output;
    }

    private void Then_I_log_something()
    {
        _output.WriteLine("custom output");
    }
}
```

**After (xUnit v3):**
```csharp
public class My_feature : FeatureFixture
{
    private void Then_I_log_something()
    {
        TestOutput.WriteLine("custom output");
    }
}
```

`TestOutput` is a property on `FeatureFixture` that works automatically — no constructor injection needed. It resolves from `TestContext.Current.TestOutputHelper` under the hood.

If you weren't using constructor injection (most projects), no change is needed.

---

## Step 7: Build and run

That's all the changes. Build your project and run:

```bash
# Via dotnet test (same as before)
dotnet test

# Or run the executable directly (new in xUnit v3)
dotnet run --project MyTests.csproj
```

---

## Summary of changes

| What | XUnit2 | XUnit3 |
|---|---|---|
| NuGet package | `LightBDD.XUnit2` + `xunit` | `LightBDD.XUnit3` + `xunit.v3` |
| Output type | Class library (default) | `Exe` (required) |
| Scope registration | `[assembly: MyScope]` | `[assembly: TestPipelineStartup(typeof(MyScope))]` |
| Scope class visibility | `internal` ok | Must be `public` |
| Scope class using | `using LightBDD.XUnit2;` | `using LightBDD.XUnit3;` + `using Xunit.v3;` |
| Parallelization | `[assembly: ClassCollectionBehavior(...)]` | Native xUnit v3 — parallel by default (see Step 4) |
| Namespace in feature files | `using LightBDD.XUnit2;` | `using LightBDD.XUnit3;` |
| Constructor injection | `FeatureFixture(ITestOutputHelper)` (obsolete) | Not available — `TestContext.Current` is used automatically |
| `[Scenario]` attribute | Same | Same |
| `[IgnoreScenario]` | Same | Same |
| `[IgnoreScenarioIf]` | Same | Same |
| `StepExecution.IgnoreScenario()` | Same | Same |
| `[Scenario(Skip = "...")]` | Same | Same |
| `[InlineData(Skip = "...")]` | Same | Same |
| `Runner.RunScenario(...)` | Same | Same |
| LightBDD reports | Same | Same |

---

## What stays exactly the same

- All `[Scenario]` methods — no code changes
- All step methods — no code changes
- `Runner.RunScenario(...)` / `Runner.RunScenarioAsync(...)` — no code changes
- `[IgnoreScenario("reason")]` — works identically
- `[IgnoreScenarioIf<T>("setting", "reason")]` — works identically
- `StepExecution.IgnoreScenario("reason")` — works identically
- `[Scenario(Skip = "reason")]` — works identically, captured in LightBDD reports
- `[InlineData(..., Skip = "reason")]` — works identically, captured in LightBDD reports
- `OnConfigure()` / `OnSetUp()` / `OnTearDown()` overrides — works identically
- All LightBDD report output (XML, Markdown, plain text) — identical format
