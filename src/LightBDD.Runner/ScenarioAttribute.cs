using System;
using Xunit;
using Xunit.Sdk;

namespace LightBDD.Runner
{
    /// <summary>
    /// Scenario attribute that should be used for XUnit framework tests, as a replacement for [Fact] and [Theory] attributes.
    /// The ScenarioAttribute represents scenario test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioAttribute : FactAttribute
    {
    }
}