using System;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomFeatureDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public CustomFeatureDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}