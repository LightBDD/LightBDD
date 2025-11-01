using System;
using LightBDD.Core.Extensibility;
using LightBDD.Runner.Implementation;
using Xunit;
using Xunit.v3;

namespace LightBDD.Runner
{
    /// <summary>
    /// Scenario attribute that should be used for XUnit framework tests, as a replacement for [Fact] and [Theory] attributes.
    /// The ScenarioAttribute represents scenario test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer($"LightBDD.Runner.Implementation.{nameof(LightBddTestCaseDiscoverer)}", "LightBDD.Runner")]
    public class ScenarioAttribute : FactAttribute, IScenarioAttribute
    {
    }
}