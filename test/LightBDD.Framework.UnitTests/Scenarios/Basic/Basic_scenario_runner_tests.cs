using LightBDD.Framework.Scenarios;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.UnitTests.Scenarios.Basic
{
    [TestFixture]
    public class Basic_scenario_runner_tests : BasicScenarioTestsBase
    {
        [Test]
        public void It_should_allow_to_run_synchronous_scenarios()
        {
            var (stepsCapture, runCapture) = ExpectBasicScenarioRun();

            Runner.RunScenario(Step_one, Step_two);

            Builder.Verify();
            Assert.That(runCapture.Value, Is.True);
            Assert.That(stepsCapture.Count, Is.EqualTo(2));
            AssertStep(stepsCapture[0], nameof(Step_one));
            AssertStep(stepsCapture[1], nameof(Step_two));
        }

        [Test]
        public async Task It_should_allow_to_run_asynchronous_scenarios()
        {
            var (stepsCapture, runCapture) = ExpectBasicScenarioRun();

            await Runner.RunScenarioAsync(Step_one_async, Step_two_async);

            Builder.Verify();
            Assert.That(runCapture.Value, Is.True);
            Assert.That(stepsCapture.Count, Is.EqualTo(2));
            AssertStep(stepsCapture[0], nameof(Step_one_async));
            AssertStep(stepsCapture[1], nameof(Step_two_async));
        }

        [Test]
        public void It_should_make_synchronous_steps_finishing_immediately_in_async_mode()
        {
            var (stepsCapture, _) = ExpectBasicScenarioRun();

            Runner.RunScenario(Step_not_throwing_exception);

            Builder.Verify();
            Assert.That(stepsCapture.Count, Is.EqualTo(1));
            Assert.True(stepsCapture[0].StepInvocation.Invoke(null, null).IsCompleted, "Synchronous step should be completed after invocation");
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

        [Test]
        public void It_should_not_support_lambdas()
        {
            Action lambda = () => { };
            var builder = new TestableScenarioBuilder<NoContext>();
            builder.AddSteps(lambda);
            AssertGeneratedStepNameException(builder.Steps.Single(), lambda.GetMethodInfo());
        }

        [Test]
        public void It_should_not_support_local_functions()
        {
            void LocalFunction() { }
            Action action = LocalFunction;
            var builder = new TestableScenarioBuilder<NoContext>();
            builder.AddSteps(LocalFunction);
            AssertGeneratedStepNameException(builder.Steps.Single(), action.GetMethodInfo());
        }

        [Test]
        public void It_should_not_support_async_lambdas()
        {
            Func<Task> lambda = async () => await Task.Yield();
            var builder = new TestableScenarioBuilder<NoContext>();
            builder.AddAsyncSteps(lambda);
            AssertGeneratedStepNameException(builder.Steps.Single(), lambda.GetMethodInfo());
        }

        [Test]
        public void It_should_not_support_async_local_functions()
        {
            async Task LocalFunction()
            {
                await Task.Yield();
            }
            Func<Task> action = LocalFunction;
            var builder = new TestableScenarioBuilder<NoContext>();
            builder.AddAsyncSteps(LocalFunction);
            AssertGeneratedStepNameException(builder.Steps.Single(), action.GetMethodInfo());
        }

        private static void AssertGeneratedStepNameException(StepDescriptor descriptor, MethodInfo methodInfo)
        {
            Assert.That(descriptor.IsValid, Is.False);
            Assert.That(descriptor.CreationException.Message,
                Is.EqualTo($"The basic step syntax does not support compiler generated methods, such as {methodInfo}, as rendered step name will be unreadable. Please either pass the step method name directly or use other methods for declaring steps."));
        }
    }
}
