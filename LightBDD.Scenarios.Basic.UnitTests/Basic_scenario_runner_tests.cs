using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.Scenarios.Basic.UnitTests
{
    public interface ITestableBddRunner : IBddRunner, ICoreBddRunner { }

    [TestFixture]
    public class Basic_scenario_runner_tests
    {
        private StepDescriptor[] _capturedSteps;
        private IScenarioRunner _mockScenarioRunner;
        private ITestableBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _capturedSteps = null;
            _mockScenarioRunner = MockRepository.GenerateStrictMock<IScenarioRunner>();
            _runner = MockRepository.GenerateStrictMock<ITestableBddRunner>();
        }

        [Test]
        public void It_should_allow_to_run_synchronous_scenarios()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunSynchronously();

            _runner.Basic().RunScenario(Step_one, Step_two);

            _runner.VerifyAllExpectations();
            _mockScenarioRunner.VerifyAllExpectations();

            Assert.That(_capturedSteps, Is.Not.Null);
            Assert.That(_capturedSteps.Length, Is.EqualTo(2));

            AssertStep(_capturedSteps[0], nameof(Step_one));
            AssertStep(_capturedSteps[1], nameof(Step_two));
        }

        [Test]
        public void It_should_make_synchronous_steps_finishing_immediately_in_async_mode()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunSynchronously();

            _runner.Basic().RunScenario(Step_not_throwing_exception);

            Assert.That(_capturedSteps, Is.Not.Null);
            Assert.That(_capturedSteps.Length, Is.EqualTo(1));

            Assert.True(_capturedSteps[0].StepInvocation.Invoke(null, null).IsCompleted, "Synchronous step should be completed after invocation");
        }

        [Test]
        public async Task It_should_allow_to_run_asynchronous_scenarios()
        {
            ExpectNewScenario();
            ExpectWithCapturedScenarioDetails();
            ExpectWithSteps();
            ExpectRunAsynchronously();

            await _runner.Basic().RunScenarioAsync(Step_one_async, Step_two_async);

            _runner.VerifyAllExpectations();
            _mockScenarioRunner.VerifyAllExpectations();

            Assert.That(_capturedSteps, Is.Not.Null);
            Assert.That(_capturedSteps.Length, Is.EqualTo(2));

            AssertStep(_capturedSteps[0], nameof(Step_one_async));
            AssertStep(_capturedSteps[1], nameof(Step_two_async));
        }

        private void AssertStep(StepDescriptor step, string expectedName)
        {
            Assert.That(step.RawName, Is.EqualTo(expectedName), nameof(step.RawName));
            Assert.That(step.Parameters, Is.Empty, nameof(step.Parameters));
            Assert.That(step.PredefinedStepType, Is.Null, nameof(step.PredefinedStepType));

            var ex = Assert.Throws<Exception>(() => step.StepInvocation.Invoke(null, null).GetAwaiter().GetResult());
            Assert.That(ex.Message, Is.EqualTo(expectedName));
        }

        #region Expectations

        private void ExpectRunSynchronously()
        {
            _mockScenarioRunner.Expect(r => r.RunSynchronously());
        }

        private void ExpectWithSteps()
        {
            _mockScenarioRunner
                .Expect(s => s.WithSteps(Arg<IEnumerable<StepDescriptor>>.Is.Anything))
                .WhenCalled(call => _capturedSteps = ((IEnumerable<StepDescriptor>)call.Arguments[0]).ToArray())
                .Return(_mockScenarioRunner);
        }

        private void ExpectWithCapturedScenarioDetails()
        {
            _mockScenarioRunner
                .Expect(s => s.WithCapturedScenarioDetails())
                .Return(_mockScenarioRunner);
        }

        private void ExpectNewScenario()
        {
            _runner
                .Expect(r => r.NewScenario())
                .Return(_mockScenarioRunner);
        }

        private void ExpectRunAsynchronously()
        {
            _mockScenarioRunner.Expect(r => r.RunAsynchronously()).Return(Task.CompletedTask);
        }

        #endregion
        #region Steps
        private void Step_one() { throw new Exception(nameof(Step_one)); }
        private void Step_two() { throw new Exception(nameof(Step_two)); }
        private void Step_not_throwing_exception() { }
        private async Task Step_one_async()
        {
            await Task.Yield();
            throw new Exception(nameof(Step_one_async));
        }
        private async Task Step_two_async()
        {
            await Task.Yield();
            throw new Exception(nameof(Step_two_async));
        }
        #endregion
    }
}
