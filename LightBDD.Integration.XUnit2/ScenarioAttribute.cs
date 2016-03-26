using System;
using Xunit;
using Xunit.Sdk;

namespace LightBDD
{
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("LightBDD.Integration.XUnit2.Customization.ScenarioTestCaseDiscoverer", "LightBDD.Integration.XUnit2")]
    public class ScenarioAttribute : FactAttribute
    {
    }
}