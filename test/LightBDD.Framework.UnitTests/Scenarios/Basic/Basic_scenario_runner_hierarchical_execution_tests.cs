using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace LightBDD.Framework.UnitTests.Scenarios.Basic
{
    [TestFixture]
    public class Basic_scenario_runner_hierarchical_execution_tests : BasicScenarioTestsBase
    {
        [Test]
        public async Task It_should_run_grouped_async_steps()
        {
            var capturedSteps = Builder.ExpectAddSteps();
            Builder.ExpectBuild();

            await Runner.RunScenarioAsync(Async_step_group);

            Assert.That(capturedSteps.Count, Is.EqualTo(1));
            Assert.That(capturedSteps[0].RawName, Is.EqualTo(nameof(Async_step_group)));

            var result = (CompositeStepResultDescriptor)await capturedSteps[0].StepInvocation.Invoke(null, null);
            var subSteps = result.SubSteps.ToArray();
            Assert.That(subSteps.Length, Is.EqualTo(2));
            AssertStep(subSteps[0], nameof(Step_one_async));
            AssertStep(subSteps[1], nameof(Step_two_async));
        }

        private async Task<CompositeStep> Async_step_group()
        {
            return CompositeStep.DefineNew()
                .AddAsyncSteps(
                    Step_one_async,
                    Step_two_async)
                .Build();
        }
    }
}