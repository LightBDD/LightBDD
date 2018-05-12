using System;

namespace LightBDD.Fixie2
{
    /// <summary>
    /// Attribute that should be applied to all test classes representing feature tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : Attribute
    {
    }
}