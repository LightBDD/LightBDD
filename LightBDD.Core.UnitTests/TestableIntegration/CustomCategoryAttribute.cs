using System;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class CustomCategoryAttribute : Attribute
    {
        public string Name { get; private set; }

        public CustomCategoryAttribute(string name)
        {
            Name = name;
        }
    }
}