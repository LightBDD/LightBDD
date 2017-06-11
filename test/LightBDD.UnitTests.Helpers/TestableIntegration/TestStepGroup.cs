using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestStepGroup : CompositeStepResultDescriptor
    {
        public TestStepGroup(params StepDescriptor[] subSteps) : this(() => null, subSteps) { }
        public TestStepGroup(Func<object> contextProvider, params StepDescriptor[] subSteps) : base(contextProvider, subSteps) { }
        public TestStepGroup(Func<object> contextProvider, IEnumerable<StepDescriptor> subSteps) : base(contextProvider, subSteps) { }

        public static TestStepGroup CreateStepGroup(params Action[] steps)
        {
            return new TestStepGroup(steps.Select(TestStep.CreateSync).ToArray());
        }

        public static TestStepGroup CreateCompositeStepGroup(params Func<TestStepGroup>[] steps)
        {
            return new TestStepGroup(steps.Select(TestStep.CreateForGroup).ToArray());
        }
    }
}