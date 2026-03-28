using System;
using LightBDD.XUnit3.Implementation.Customization;
using Xunit;
using Xunit.v3;

namespace LightBDD.XUnit3
{
    /// <summary>
    /// Scenario attribute that should be used for xUnit v3 framework tests, as a replacement for [Fact] and [Theory] attributes.
    /// The ScenarioAttribute represents scenario test method and supports parameterized scenarios via [InlineData] and other data attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer(typeof(ScenarioTestCaseDiscoverer))]
    public class ScenarioAttribute : FactAttribute, ITheoryAttribute
    {
        /// <inheritdoc />
        public bool DisableDiscoveryEnumeration { get; set; }

        /// <inheritdoc />
        public bool SkipTestWithoutData { get; set; }
    }
}
