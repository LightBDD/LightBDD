using NUnit.Framework;

namespace LightBDD
{
    /// <summary>
    /// Scenario attribute that should be used for NUnit 3 framework tests, as a replacement for [Test] attribute.
    /// The ScenarioAttribute represents scenario test method.
    /// </summary>
    public class ScenarioAttribute : TestAttribute
    {
    }
}