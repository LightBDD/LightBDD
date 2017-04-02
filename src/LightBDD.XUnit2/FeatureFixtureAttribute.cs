using System;
using Xunit.Sdk;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// Attribute that should be applied to all test classes representing feature tests.
    /// For XUnit integration this attribute is meaningless and it is present for backward compatibility.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : BeforeAfterTestAttribute
    {
    }
}