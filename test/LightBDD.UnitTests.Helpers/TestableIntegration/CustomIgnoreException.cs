using System;
using LightBDD.Core.Execution;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    //TODO: review
    public class CustomIgnoreException : IgnoreException
    {
        public CustomIgnoreException(string reason) : base(reason) { }
    }
}