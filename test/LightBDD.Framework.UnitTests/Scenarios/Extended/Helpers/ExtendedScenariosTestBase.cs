using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class ExtendedScenariosTestBase<T> : Steps
    {
        protected StepDescriptor[] CapturedSteps;
        protected Mock<ICoreScenarioBuilder> MockScenarioRunner;
        protected IBddRunner<T> Runner;

        [SetUp]
        public void SetUp()
        {
            CapturedSteps = null;
            MockScenarioRunner = new Mock<ICoreScenarioBuilder>();
            Runner = new MockBddRunner<T>(TestableIntegrationContextBuilder.Default().Build().Configuration, MockScenarioRunner.Object);
        }

        protected void ExpectSynchronousScenarioRun()
        {
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunScenario();
        }

        protected void ExpectAsynchronousScenarioRun()
        {
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunScenarioAsync();
        }

        protected void ExpectRunScenario()
        {
            MockScenarioRunner
                .Setup(r => r.RunScenario())
                .Verifiable();
        }

        protected void ExpectWithSteps()
        {
            MockScenarioRunner
                .Setup(s => s.AddSteps(It.IsAny<IEnumerable<StepDescriptor>>()))
                .Returns((IEnumerable<StepDescriptor> steps) =>
                {
                    CapturedSteps = steps.ToArray();
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

        protected void ExpectRunScenarioAsync()
        {
            MockScenarioRunner
                .Setup(r => r.RunScenarioAsync())
                .Returns(Task.FromResult(0))
                .Verifiable();
        }

        protected void VerifyExpectations()
        {
            MockScenarioRunner.VerifyAll();
        }

        protected void AssertStep(StepDescriptor step, string expectedName, string expectedPredefinedStepType = null)
        {
            Assert.That(step.RawName, Is.EqualTo(expectedName), nameof(step.RawName));
            Assert.That(step.Parameters, Is.Empty, nameof(step.Parameters));
            Assert.That(step.PredefinedStepType, Is.EqualTo(expectedPredefinedStepType), nameof(step.PredefinedStepType));

            var ex = Assert.Throws<ScenarioExecutionException>(() => step.StepInvocation.Invoke(null, null).GetAwaiter().GetResult());
            Assert.That(ex.InnerException, Is.TypeOf<Exception>());
            Assert.That(ex.InnerException.Message, Is.EqualTo(expectedName));
        }
    }
}