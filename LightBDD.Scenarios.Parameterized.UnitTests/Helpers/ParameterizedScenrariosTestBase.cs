using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.Scenarios.Parameterized.UnitTests.Helpers
{
    public class ParameterizedScenrariosTestBase<T> : Steps
    {
        protected StepDescriptor[] CapturedSteps;
        protected IScenarioRunner MockScenarioRunner;
        protected ITestableBddRunner<T> Runner;

        [SetUp]
        public void SetUp()
        {
            CapturedSteps = null;
            MockScenarioRunner = MockRepository.GenerateStrictMock<IScenarioRunner>();
            Runner = MockRepository.GenerateStrictMock<ITestableBddRunner<T>>();
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
            MockScenarioRunner.Expect(r => r.RunSynchronously());
        }

        protected void ExpectWithSteps()
        {
            MockScenarioRunner
                .Expect(s => s.WithSteps(Arg<IEnumerable<StepDescriptor>>.Is.Anything))
                .WhenCalled(call => CapturedSteps = ((IEnumerable<StepDescriptor>)call.Arguments[0]).ToArray())
                .Return(MockScenarioRunner);
        }

        protected void ExpectWithCapturedScenarioDetails()
        {
            MockScenarioRunner
                .Expect(s => s.WithCapturedScenarioDetails())
                .Return(MockScenarioRunner);
        }

        protected void ExpectNewScenario()
        {
            Runner
                .Expect(r => r.NewScenario())
                .Return(MockScenarioRunner);
        }

        protected void ExpectRunAsynchronously()
        {
            MockScenarioRunner.Expect(r => r.RunAsynchronously()).Return(Task.CompletedTask);
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