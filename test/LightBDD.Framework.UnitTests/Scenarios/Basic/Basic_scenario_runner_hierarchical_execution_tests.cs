using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Basic
{
    [TestFixture]
    public class Basic_scenario_runner_hierarchical_execution_tests : BasicScenarioTestsBase
    {
        [Test]
        public async Task It_should_run_grouped_async_steps()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunAsynchronously();

            await Runner.RunScenarioAsync(Async_step_group);

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(1));
            Assert.That(CapturedSteps[0].RawName, Is.EqualTo(nameof(Async_step_group)));

            var result = (CompositeStepResultDescriptor)await CapturedSteps[0].StepInvocation.Invoke(null, null);
            var subSteps = result.SubSteps.ToArray();
            Assert.That(subSteps.Length, Is.EqualTo(2));
            AssertStep(subSteps[0], nameof(Step_one_async));
            AssertStep(subSteps[1], nameof(Step_two_async));
        }

        private async Task<StepGroup> Async_step_group()
        {
            return StepGroup.DefineNew()
                .AddSteps(
                    Step_one_async,
                    Step_two_async)
                .Build();
        }
    }
}