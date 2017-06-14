using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestCompositeStep : CompositeStepResultDescriptor
    {
        public TestCompositeStep(params StepDescriptor[] subSteps) : this(() => null, subSteps) { }
        public TestCompositeStep(Func<object> contextProvider, params StepDescriptor[] subSteps) : base(contextProvider, subSteps) { }
        public TestCompositeStep(Func<object> contextProvider, IEnumerable<StepDescriptor> subSteps) : base(contextProvider, subSteps) { }

        public static TestCompositeStep Create(params Action[] steps)
        {
            return new TestCompositeStep(steps.Select(TestStep.CreateSync).ToArray());
        }

        public static TestCompositeStep CreateFromComposites(params Func<TestCompositeStep>[] steps)
        {
            return new TestCompositeStep(steps.Select(TestStep.CreateForGroup).ToArray());
        }
    }
}