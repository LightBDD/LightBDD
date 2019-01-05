using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using NUnit.Framework;
using System.Threading.Tasks;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended
{
    [TestFixture]
    public class Extended_scenario_runner_step_type_parsing_tests : ExtendedScenariosTestBase<NoContext>
    {
        [Test]
        public void It_should_not_capture_single_character_but_everything_else_for_step_type_in_synchronous_run()
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();

            Runner.RunScenario(
                _ => Step_one(),
                x => Step_one(),
                when => Step_two(),
                xx => Step_two()
                );

            Builder.Verify();

            Assert.That(stepsCapture.Count, Is.EqualTo(4));

            AssertStep(stepsCapture[0], nameof(Step_one));
            AssertStep(stepsCapture[1], nameof(Step_one));
            AssertStep(stepsCapture[2], nameof(Step_two), "when");
            AssertStep(stepsCapture[3], nameof(Step_two), "xx");
        }

        [Test]
        public async Task It_should_not_capture_single_character_but_everything_else_for_step_type_in_asynchronous_run()
        {
            var (stepsCapture, _) = ExpectExtendedScenarioRun();

            await Runner.RunScenarioAsync(
                _ => Step_one_async(),
                x => Step_one_async(),
                when => Step_two_async(),
                xx => Step_two_async()
                );

            Builder.Verify();

            Assert.That(stepsCapture.Count, Is.EqualTo(4));

            AssertStep(stepsCapture[0], nameof(Step_one_async));
            AssertStep(stepsCapture[1], nameof(Step_one_async));
            AssertStep(stepsCapture[2], nameof(Step_two_async), "when");
            AssertStep(stepsCapture[3], nameof(Step_two_async), "xx");
        }
    }
}