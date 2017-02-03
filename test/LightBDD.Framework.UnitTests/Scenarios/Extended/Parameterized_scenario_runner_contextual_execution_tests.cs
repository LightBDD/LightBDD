using System.Linq;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Scenarios.Extended.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Scenarios.Extended.UnitTests
{
    [TestFixture]
    public class Parameterized_scenario_runner_contextual_execution_tests : ParameterizedScenariosTestBase<MyContext>
    {

        [Test]
        public void It_should_allow_executing_steps_and_parameters_with_context_in_sync_mode()
        {
            ExpectSynchronousScenarioRun();

            var context = new MyContext();
            Runner.RunScenario(
                ctx => ctx.AssertIsSameAs(context), //passing context to step
                ctx => ctx.AssertIsSameAs(ctx)); // passing context to step and step argument

            foreach (var step in CapturedSteps)
                Assert.DoesNotThrow(() => step.StepInvocation(context, step.Parameters.Select(p => p.ValueEvaluator(context)).ToArray()));
        }

        [Test]
        public async Task It_should_allow_executing_steps_and_parameters_with_context_in_async_mode()
        {
            ExpectAsynchronousScenarioRun();

            var context = new MyContext();
            await Runner.RunScenarioAsync(
                ctx => ctx.AssertIsSameAsAsync(context), //passing context to step
                ctx => ctx.AssertIsSameAsAsync(ctx)); // passing context to step and step argument

            foreach (var step in CapturedSteps)
                Assert.DoesNotThrowAsync(() => step.StepInvocation(context, step.Parameters.Select(p => p.ValueEvaluator(context)).ToArray()));
        }
    }
}