using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
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
            var capturedSteps = Builder.ExpectAddSteps();
            Builder.ExpectBuild();

            Runner.RunScenario(
                _ => Step_one(),
                x => Step_one(),
                when => Step_two(),
                xx => Step_two()
                );

            Builder.Verify();

            Assert.That(capturedSteps.Count, Is.EqualTo(4));

            AssertStep(capturedSteps[0], nameof(Step_one));
            AssertStep(capturedSteps[1], nameof(Step_one));
            AssertStep(capturedSteps[2], nameof(Step_two), "when");
            AssertStep(capturedSteps[3], nameof(Step_two), "xx");
        }

        [Test]
        public async Task It_should_not_capture_single_character_but_everything_else_for_step_type_in_asynchronous_run()
        {
            var capturedSteps = Builder.ExpectAddSteps();
            Builder.ExpectBuild();

            await Runner.RunScenarioAsync(
                _ => Step_one_async(),
                x => Step_one_async(),
                when => Step_two_async(),
                xx => Step_two_async()
                );

            Builder.Verify();

            Assert.That(capturedSteps.Count, Is.EqualTo(4));

            AssertStep(capturedSteps[0], nameof(Step_one_async));
            AssertStep(capturedSteps[1], nameof(Step_one_async));
            AssertStep(capturedSteps[2], nameof(Step_two_async), "when");
            AssertStep(capturedSteps[3], nameof(Step_two_async), "xx");
        }
    }
}