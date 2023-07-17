using System;
using LightBDD.Core.Execution;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    //TODO: review
    public class CustomIgnoreException : IgnoreScenarioException
    {
        public CustomIgnoreException(string reason) : base(reason) { }
    }
}