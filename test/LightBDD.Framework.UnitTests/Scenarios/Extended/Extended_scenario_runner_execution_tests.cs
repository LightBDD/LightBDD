using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended
{
    [TestFixture]
    public class Extended_scenario_runner_execution_tests : ExtendedScenariosTestBase<NoContext>
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

            var ex = Assert.Throws<ScenarioExecutionException>(() => step.StepInvocation(null, step.Parameters.Select(p => p.ValueEvaluator(null)).ToArray()));
            Assert.That(ex.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.InnerException.Message, Is.EqualTo(ExceptionMessageForStep_with_parameters(32)));
        }

        [Test]
        public async Task It_should_allow_to_execute_parameterized_steps_in_async_mode()
        {
            ExpectAsynchronousScenarioRun();
            await Runner.RunScenarioAsync(_ => Step_with_parameters_async(33, "33"));
            var step = CapturedSteps.Single();

            var ex = Assert.ThrowsAsync<ScenarioExecutionException>(() => step.StepInvocation(null, step.Parameters.Select(p => p.ValueEvaluator(null)).ToArray()));
            Assert.That(ex.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.InnerException.Message, Is.EqualTo(ExceptionMessageForStep_with_parameters(33)));
        }

        [Test]
        public void It_should_allow_to_execute_parameterized_steps_in_sync_mode_fluent_way()
        {
            var builder = new TestableScenarioBuilder<NoContext>();
            builder.AddSteps(_ => Step_with_parameters(32, "32"));
            var step = builder.Steps.Single();

            var ex = Assert.Throws<ScenarioExecutionException>(() => step.StepInvocation(null, step.Parameters.Select(p => p.ValueEvaluator(null)).ToArray()));
            Assert.That(ex.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.InnerException.Message, Is.EqualTo(ExceptionMessageForStep_with_parameters(32)));
        }

        [Test]
        public void It_should_allow_to_execute_parameterized_steps_in_async_mode_fluent_way()
        {
            var builder = new TestableScenarioBuilder<NoContext>();
            builder.AddAsyncSteps(_ => Step_with_parameters_async(33, "33"));
            var step = builder.Steps.Single();

            var ex = Assert.ThrowsAsync<ScenarioExecutionException>(() => step.StepInvocation(null, step.Parameters.Select(p => p.ValueEvaluator(null)).ToArray()));
            Assert.That(ex.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(ex.InnerException.Message, Is.EqualTo(ExceptionMessageForStep_with_parameters(33)));
        }

        private async void Step_one_async_void()
        {
            await Task.Delay(200);
        }
    }
}