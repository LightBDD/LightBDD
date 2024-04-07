using System;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{   
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomScenarioDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public CustomScenarioDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
