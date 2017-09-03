using System;
using Xunit;
using Xunit.Sdk;
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

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
        /// <summary>
        /// Marks the test so that it will not be run, and gets or sets the skip reason
        /// </summary>
        [Obsolete("Please use IgnoreScenarioAttribute on scenario or step method instead, as it will make test appearing in the reports.")]
        public override string Skip
        {
            get => base.Skip;
            set => base.Skip = value;
        }
    }
}