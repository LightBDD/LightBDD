using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class ParameterizedScenariosTestBase<T> : Steps
    {
        protected StepDescriptor[] CapturedSteps;
        protected Mock<IScenarioRunner> MockScenarioRunner;
        protected Mock<ITestableBddRunner<T>> MockRunner;
        protected ITestableBddRunner<T> Runner => MockRunner.Object;

        [SetUp]
        public void SetUp()
        {
            CapturedSteps = null;
            MockScenarioRunner = new Mock<IScenarioRunner>();
            MockRunner = new Mock<ITestableBddRunner<T>>();
        }

        protected void ExpectSynchronousScenarioRun()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunSynchronously();
        }

        protected void ExpectAsynchronousScenarioRun()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunAsynchronously();
        }

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
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void VerifyExpectations()
        {
            MockScenarioRunner.VerifyAll();
            MockRunner.VerifyAll();
        }

        protected void AssertStep(StepDescriptor step, string expectedName, string expectedPredefinedStepType = null)
        {
            Assert.That(step.RawName, Is.EqualTo(expectedName), nameof(step.RawName));
            Assert.That(step.Parameters, Is.Empty, nameof(step.Parameters));
            Assert.That(step.PredefinedStepType, Is.EqualTo(expectedPredefinedStepType), nameof(step.PredefinedStepType));

            var ex = Assert.Throws<Exception>(() => step.StepInvocation.Invoke(null, null).GetAwaiter().GetResult());
            Assert.That(ex.Message, Is.EqualTo(expectedName));
        }
    }
}