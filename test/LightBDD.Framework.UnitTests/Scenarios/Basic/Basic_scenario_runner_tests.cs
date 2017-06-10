using System;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Basic
{
    [TestFixture]
    public class Basic_scenario_runner_tests : BasicScenarioTestsBase
    {
        [Test]
        public void It_should_allow_to_run_synchronous_scenarios()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunSynchronously();

            Runner.RunScenario(Step_one, Step_two);

            MockRunner.Verify();
            MockScenarioRunner.Verify();

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(2));

            AssertStep(CapturedSteps[0], nameof(Step_one));
            AssertStep(CapturedSteps[1], nameof(Step_two));
        }

        [Test]
        public void It_should_make_synchronous_steps_finishing_immediately_in_async_mode()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunSynchronously();

            Runner.RunScenario(Step_not_throwing_exception);

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(1));

            Assert.True(CapturedSteps[0].StepInvocation.Invoke(null, null).IsCompleted, "Synchronous step should be completed after invocation");
        }

        [Test]
        public async Task It_should_allow_to_run_asynchronous_scenarios()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunAsynchronously();

            await Runner.RunScenarioAsync(Step_one_async, Step_two_async);

            MockRunner.Verify();
            MockScenarioRunner.Verify();

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(2));

            AssertStep(CapturedSteps[0], nameof(Step_one_async));
            AssertStep(CapturedSteps[1], nameof(Step_two_async));
        }

        [Test]
        public async Task It_should_allow_to_run_void_and_async_void_steps_in_asynchronous_mode()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunAsynchronously();

            await Runner.RunScenarioActionsAsync(Step_one_async_void, Step_two);

            MockRunner.Verify();
            MockScenarioRunner.Verify();
        }
    }
}
