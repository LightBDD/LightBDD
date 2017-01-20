using System;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class CustomIgnoreException : Exception
    {
        public CustomIgnoreException(string reason) : base(reason) { }
    }
}