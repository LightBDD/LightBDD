using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Scenarios.Extended.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Scenarios.Extended.UnitTests
{
    [TestFixture]
    public class Parameterized_scenario_runner_execution_tests : ParameterizedScenariosTestBase<NoContext>
    {
        [Test]
        public void It_should_make_synchronous_steps_finishing_immediately_in_async_mode()
        {
            ExpectSynchronousScenarioRun();

            Runner.RunScenario(_ => Step_not_throwing_exception());

            Assert.That(CapturedSteps, Is.Not.Null);
            Assert.That(CapturedSteps.Length, Is.EqualTo(1));

            Assert.True(CapturedSteps[0].StepInvocation.Invoke(null, null).IsCompleted, "Synchronous step should be completed after invocation");
        }

        [Test]
        public void It_should_allow_to_execute_parameterized_steps_in_sync_mode()
        {
            ExpectSynchronousScenarioRun();
            Runner.RunScenario(_ => Step_with_parameters(32, "32"));
            var step = CapturedSteps.Single();

            var ex = Assert.Throws<InvalidOperationException>(() => step.StepInvocation(null, step.Parameters.Select(p => p.ValueEvaluator(null)).ToArray()));
            Assert.That(ex.Message, Is.EqualTo(ExceptionMessageForStep_with_parameters(32)));
        }

        [Test]
        public async Task It_should_allow_to_execute_parameterized_steps_in_async_mode()
        {
            ExpectAsynchronousScenarioRun();
            await Runner.RunScenarioAsync(_ => Step_with_parameters_async(33, "33"));
            var step = CapturedSteps.Single();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => step.StepInvocation(null, step.Parameters.Select(p => p.ValueEvaluator(null)).ToArray()));
            Assert.That(ex.Message, Is.EqualTo(ExceptionMessageForStep_with_parameters(33)));
        }
    }
}