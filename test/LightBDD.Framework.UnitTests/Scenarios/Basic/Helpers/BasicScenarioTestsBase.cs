using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers
{
    public class BasicScenarioTestsBase
    {
        protected Mock<ICoreScenarioBuilder> Builder;
        protected IBddRunner Runner;

        [SetUp]
        public void SetUp()
        {
            Builder = ScenarioMocks.CreateScenarioBuilder();
            Runner = ScenarioMocks.CreateBddRunner(Builder);
        }

        protected (List<StepDescriptor> scenarioCapture, Capture<bool> runCapture) ExpectBasicScenarioRun()
        {
            var stepsCapture = Builder.ExpectAddSteps();
            Builder.ExpectWithCapturedScenarioDetailsIfNotSpecified();
            var runCapture = Builder.ExpectBuild();
            return (stepsCapture, runCapture);
        }

        protected void AssertStep(StepDescriptor step, string expectedName)
        {
            Assert.That(step.RawName, Is.EqualTo(expectedName), nameof(step.RawName));
            Assert.That(step.Parameters, Is.Empty, nameof(step.Parameters));
            Assert.That(step.PredefinedStepType, Is.Null, nameof(step.PredefinedStepType));

            var ex = Assert.Throws<ScenarioExecutionException>(() => step.StepInvocation.Invoke(null, null).GetAwaiter().GetResult());
            Assert.That(ex.InnerException, Is.TypeOf<Exception>());
            Assert.That(ex.InnerException?.Message, Is.EqualTo(expectedName));
        }

        #region Steps

        protected void Step_one() { throw new Exception(nameof(Step_one)); }
        protected void Step_two() { throw new Exception(nameof(Step_two)); }
        protected void Step_not_throwing_exception() { }

        protected async Task Step_one_async()
        {
            await Task.Yield();
            throw new Exception(nameof(Step_one_async));
        }

        protected async void Step_one_async_void()
        {
            await Task.Delay(200);
        }

        protected async Task Step_two_async()
        {
            await Task.Yield();
            throw new Exception(nameof(Step_two_async));
        }
        #endregion
    }
}