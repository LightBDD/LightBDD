using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers
{
    public class BasicScenarioTestsBase
    {
        public interface ITestableBddRunner : IBddRunner, IFeatureFixtureRunner { }
        protected StepDescriptor[] CapturedSteps;
        protected Mock<IScenarioRunner> MockScenarioRunner;
        protected Mock<ITestableBddRunner> MockRunner;
        protected ITestableBddRunner Runner => MockRunner.Object;

        [SetUp]
        public void SetUp()
        {
            CapturedSteps = null;
            MockScenarioRunner = new Mock<IScenarioRunner>();
            MockRunner = new Mock<ITestableBddRunner>();
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

        #region Expectations

        protected void ExpectRunSynchronously()
        {
            MockScenarioRunner
                .Setup(r => r.RunSynchronously())
                .Verifiable();
        }

        protected void ExpectWithSteps()
        {
            MockScenarioRunner
                .Setup(s => s.WithSteps(It.IsAny<IEnumerable<StepDescriptor>>()))
                .Returns((IEnumerable<StepDescriptor> desc) =>
                {
                    CapturedSteps = desc.ToArray();
                    return MockScenarioRunner.Object;
                })
                .Verifiable();
        }

        protected void ExpectWithCapturedScenarioDetails()
        {
            MockScenarioRunner
                .Setup(s => s.WithCapturedScenarioDetails())
                .Returns(MockScenarioRunner.Object)
                .Verifiable();
        }

        protected void ExpectNewScenario()
        {
            MockRunner
                .Setup(r => r.NewScenario())
                .Returns(MockScenarioRunner.Object)
                .Verifiable();
        }

        protected void ExpectRunAsynchronously()
        {
            MockScenarioRunner
                .Setup(r => r.RunAsynchronously())
                .Returns(Task.FromResult(0))
                .Verifiable();
        }

        protected void ExpectSynchronousExecution()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunSynchronously();
        }

        protected void ExpectAsynchronousExecution()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunAsynchronously();
        }

        protected void VerifyAllExpectations()
        {
            MockRunner.Verify();
            MockScenarioRunner.Verify();
        }
        #endregion
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