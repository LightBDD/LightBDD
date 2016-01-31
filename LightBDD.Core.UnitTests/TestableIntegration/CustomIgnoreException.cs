using System;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class CustomIgnoreException : Exception
    {
        public CustomIgnoreException(string reason) : base(reason) { }
    }
}