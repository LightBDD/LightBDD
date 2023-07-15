using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Discovery;
using Shouldly;

namespace LightBDD.Core.UnitTests.Helpers;

static class ScenarioCaseExtensions
{
    public static void AssertEquivalentTo(this IEnumerable<ScenarioCase> actual, params ScenarioCase[] expected)
    {
        actual.Select(Format).ToArray().ShouldBe(expected.Select(Format).ToArray(), true);
    }

    private static string Format(ScenarioCase s) => $"{s.FeatureFixtureType}|{s.ScenarioMethod}|{s.RequireArgumentResolutionAtRuntime}|{string.Join(",", s.ScenarioArguments)}";
}