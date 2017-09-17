using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace LightBDD.Framework.UnitTests.Scenarios.Extended
{
    [TestFixture]
    public class Parameterized_scenario_runner_hierarchical_execution_tests : ParameterizedScenariosTestBase<NoContext>
    {
        [Test]
        public async Task It_should_run_grouped_steps()
        {
            ExpectSynchronousScenarioRun();

            Runner.RunScenario(_ => Step_group());

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(1));
            Assert.That(CapturedSteps[0].RawName, Is.EqualTo(nameof(Step_group)));

            var result = (CompositeStepResultDescriptor)await CapturedSteps[0].StepInvocation.Invoke(null, null);
            var subSteps = result.SubSteps.ToArray();
            Assert.That(subSteps.Length, Is.EqualTo(2));
            AssertStep(subSteps[0], nameof(Step_one));
            AssertStep(subSteps[1], nameof(Step_two));
        }

        [Test]
        public async Task It_should_run_grouped_async_steps()
        {
            ExpectAsynchronousScenarioRun();

            await Runner.RunScenarioAsync(_ => Async_step_group());

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(1));
            Assert.That(CapturedSteps[0].RawName, Is.EqualTo(nameof(Async_step_group)));

            var result = (CompositeStepResultDescriptor)await CapturedSteps[0].StepInvocation.Invoke(null, null);
            var subSteps = result.SubSteps.ToArray();
            Assert.That(subSteps.Length, Is.EqualTo(2));
            AssertStep(subSteps[0], nameof(Step_one_async));
            AssertStep(subSteps[1], nameof(Step_two_async));
        }

        [Test]
        public void It_should_defer_step_parsing_to_execution()
        {
            CompositeStep group = null;
            Assert.DoesNotThrow(() => group = new TestableCompositeStepBuilder()
                .AddAsyncSteps(_ => null as Task)
                .Build());

            Assert.Throws<ArgumentException>(() => group.SubSteps.ToArray());
        }


        private async Task<CompositeStep> Async_step_group()
        {
            return new TestableCompositeStepBuilder()
                .AddAsyncSteps(
                    _ => Step_one_async(),
                    _ => Step_two_async())
                .Build();
        }

        private CompositeStep Step_group()
        {
            return new TestableCompositeStepBuilder()
                .AddSteps(
                    _ => Step_one(),
                    _ => Step_two())
                .Build();
        }
    }
}