using System;
using Xunit;
using Xunit.Sdk;

namespace LightBDD
{
    /// <summary>
    /// Scenario attribute that should be used for xUnit tests, as a replacement for [Fact] and [Theory] attributes.
    /// The Scenario attribute works for parameterized and parameter-less tests and it supports <c>ScenarioAssert.Ignore</c>() method, alowing to skip test execution at runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("LightBDD.XUnit.ScenarioTestCaseDiscoverer", "LightBDD.XUnit")]
    public class ScenarioAttribute : FactAttribute
    {
    }
}