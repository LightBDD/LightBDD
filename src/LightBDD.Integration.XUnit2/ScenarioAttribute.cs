using System;
using Xunit;
using Xunit.Sdk;

namespace LightBDD
{
    /// <summary>
    /// Scenario attribute that should be used for XUnit framework tests, as a replacement for [Fact] and [Theory] attributes.
    /// The ScenarioAttribute represents scenario test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("LightBDD.Integration.XUnit2.Customization.ScenarioTestCaseDiscoverer", "LightBDD.Integration.XUnit2")]
    public class ScenarioAttribute : FactAttribute
    {
    }
}