using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;

namespace LightBDD.ScenarioHelpers
{
    public class TestCompositeStep : CompositeStepResultDescriptor
    {
        public TestCompositeStep(params StepDescriptor[] subSteps) : this(() => null, subSteps) { }
        public TestCompositeStep(Func<object?> contextProvider, params StepDescriptor[] subSteps)
            : base(Resolvable.Use(contextProvider, false), subSteps) { }
        public TestCompositeStep(Func<object?> contextProvider, IEnumerable<StepDescriptor> subSteps)
            : base(Resolvable.Use(contextProvider, false), subSteps) { }
        public TestCompositeStep(Resolvable<object?> contextDescriptor, params StepDescriptor[] subSteps)
            : base(contextDescriptor, subSteps) { }

        public static TestCompositeStep Create(params Action[] steps)
        {
            return new TestCompositeStep(steps.Select(TestStep.CreateSync).ToArray());
        }

        public static TestCompositeStep CreateFromComposites(params Func<TestCompositeStep>[] steps)
        {
            return new TestCompositeStep(steps.Select(TestStep.CreateComposite).ToArray());
        }
    }
}