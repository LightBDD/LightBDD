using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class ExtendedScenariosTestBase<T> : Steps
    {
        protected Mock<ICoreScenarioStepsRunner> Builder;
        protected IBddRunner<T> Runner;

        [SetUp]
        public void SetUp()
        {
            Builder = ScenarioMocks.CreateScenarioBuilder();
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();
            Builder.SetupConfiguration(configuration);
            Runner = new MockBddRunner<T>(configuration, Builder.Object);
        }

        protected (List<StepDescriptor> scenarioCapture, Capture<bool> runCapture) ExpectExtendedScenarioRun()
        {
            var stepsCapture = Builder.ExpectAddSteps();
            var runCapture = Builder.ExpectRun();
            return (stepsCapture, runCapture);
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