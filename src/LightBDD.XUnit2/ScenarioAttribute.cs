using System;
using Xunit;
using Xunit.Sdk;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// Scenario attribute that should be used for XUnit framework tests, as a replacement for [Fact] and [Theory] attributes.
    /// The ScenarioAttribute represents scenario test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("LightBDD.XUnit2.Implementation.Customization.ScenarioTestCaseDiscoverer", "LightBDD.XUnit2")]
    public class ScenarioAttribute : FactAttribute
    {
    }
}