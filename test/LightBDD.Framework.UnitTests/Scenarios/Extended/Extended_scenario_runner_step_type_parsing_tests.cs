using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended
{
    [TestFixture]
    public class Extended_scenario_runner_step_type_parsing_tests : ExtendedScenariosTestBase<NoContext>
    {
        [Test]
        public void It_should_not_capture_single_character_but_everything_else_for_step_type_in_synchronous_run()
        {
            ExpectSynchronousScenarioRun();

            Runner.RunScenario(
                _ => Step_one(),
                x => Step_one(),
                when => Step_two(),
                xx => Step_two()
                );

            MockScenarioRunner.Verify();

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(4));

            AssertStep(CapturedSteps[0], nameof(Step_one));
            AssertStep(CapturedSteps[1], nameof(Step_one));
            AssertStep(CapturedSteps[2], nameof(Step_two), "when");
            AssertStep(CapturedSteps[3], nameof(Step_two), "xx");
        }

        [Test]
        public async Task It_should_not_capture_single_character_but_everything_else_for_step_type_in_asynchronous_run()
        {
            ExpectAsynchronousScenarioRun();

            await Runner.RunScenarioAsync(
                _ => Step_one_async(),
                x => Step_one_async(),
                when => Step_two_async(),
                xx => Step_two_async()
                );

            MockScenarioRunner.Verify();

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(4));

            AssertStep(CapturedSteps[0], nameof(Step_one_async));
            AssertStep(CapturedSteps[1], nameof(Step_one_async));
            AssertStep(CapturedSteps[2], nameof(Step_two_async), "when");
            AssertStep(CapturedSteps[3], nameof(Step_two_async), "xx");
        }
    }
}