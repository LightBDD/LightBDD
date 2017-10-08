using System;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class CustomCategoryAttribute : Attribute
    {
        public string Name { get; }

        public CustomCategoryAttribute(string name)
        {
            Name = name;
        }
    }
}