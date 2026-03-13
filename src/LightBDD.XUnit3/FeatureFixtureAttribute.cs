using System;
using Xunit.v3;

namespace LightBDD.XUnit3
{
    /// <summary>
    /// Attribute that should be applied to all test classes representing feature tests.
    /// For xUnit v3 integration this attribute is meaningless and it is present for backward compatibility.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : BeforeAfterTestAttribute
    {
    }
}
