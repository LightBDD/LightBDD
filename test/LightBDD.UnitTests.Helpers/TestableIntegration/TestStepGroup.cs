using System.Collections.Generic;
using LightBDD.Core.Extensibility;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestStepGroup : StepResultDescriptor
    {
        public TestStepGroup(IEnumerable<StepDescriptor> subSteps) : base(subSteps)
        {
        }
    }
}