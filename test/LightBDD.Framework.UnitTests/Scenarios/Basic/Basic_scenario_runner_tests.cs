using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;
using System.Threading.Tasks;

namespace LightBDD.Framework.UnitTests.Scenarios.Basic
{
    [TestFixture]
    public class Basic_scenario_runner_tests : BasicScenarioTestsBase
    {
        [Test]
        public void It_should_allow_to_run_synchronous_scenarios()
        {
            var capturedSteps = Builder.ExpectAddSteps();
            var capturedRun = Builder.ExpectBuild();

            Runner.RunScenario(Step_one, Step_two);

            Assert.That(capturedRun.Value, Is.True);
            Assert.That(capturedSteps.Count, Is.EqualTo(2));
            AssertStep(capturedSteps[0], nameof(Step_one));
            AssertStep(capturedSteps[1], nameof(Step_two));
        }

        [Test]
        public async Task It_should_allow_to_run_asynchronous_scenarios()
        {
            var capturedSteps = Builder.ExpectAddSteps();
            var capturedRun = Builder.ExpectBuild();

            await Runner.RunScenarioAsync(Step_one_async, Step_two_async);

            Assert.That(capturedRun.Value, Is.True);
            Assert.That(capturedSteps.Count, Is.EqualTo(2));
            AssertStep(capturedSteps[0], nameof(Step_one_async));
            AssertStep(capturedSteps[1], nameof(Step_two_async));
        }

        [Test]
        public void It_should_make_synchronous_steps_finishing_immediately_in_async_mode()
        {
            var capturedSteps = Builder.ExpectAddSteps();
            Builder.ExpectBuild();

            Runner.RunScenario(Step_not_throwing_exception);

            Assert.That(capturedSteps.Count, Is.EqualTo(1));
            Assert.True(capturedSteps[0].StepInvocation.Invoke(null, null).IsCompleted, "Synchronous step should be completed after invocation");
        }

        [Test]
        public void It_should_allow_to_run_sync_scenarios_in_fluent_way()
        {
            var builder = new TestableScenarioBuilder<NoContext>();
            builder.AddSteps(Step_one, Step_two);

            var steps = builder.Steps;

            Assert.That(steps, Is.Not.Null);
            Assert.That(steps.Count, Is.EqualTo(2));

            AssertStep(steps[0], nameof(Step_one));
            AssertStep(steps[1], nameof(Step_two));
        }

        [Test]
        public void It_should_allow_to_run_async_scenarios_in_fluent_way()
        {
            var builder = new TestableScenarioBuilder<NoContext>();
            builder.AddAsyncSteps(Step_one_async, Step_two_async);

            var steps = builder.Steps;

            Assert.That(steps, Is.Not.Null);
            Assert.That(steps.Count, Is.EqualTo(2));

            AssertStep(steps[0], nameof(Step_one_async));
            AssertStep(steps[1], nameof(Step_two_async));
        }
    }
}
