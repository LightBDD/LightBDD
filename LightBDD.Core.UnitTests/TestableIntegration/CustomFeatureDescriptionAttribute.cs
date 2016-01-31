using System;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomFeatureDescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public CustomFeatureDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}