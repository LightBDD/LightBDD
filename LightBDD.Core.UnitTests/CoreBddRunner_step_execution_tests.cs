using System;
using System.Collections.Generic;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_step_execution_tests
    {
        private IBddRunner _runner;
        private List<string> _executedSteps;
        private IFeatureBddRunner _feature;

        [SetUp]
        public void SetUp()
        {
            _feature = TestableBddRunnerFactory.GetRunner(GetType());
            _runner = _feature.GetRunner(this);
            _executedSteps = new List<string>();
        }

        [Test]
        public void Runner_should_execute_all_steps()
        {
            _runner.Test().TestScenario(
                Given_step_one,
                When_step_two,
                Then_step_three);

            Assert.That(_executedSteps, Is.EqualTo(new[] { "Given_step_one", "When_step_two", "Then_step_three" }));
        }

        [Test]
        public void Runner_should_propagate_step_exception_and_stop_executing_further_steps()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _runner.Test().TestScenario(
                Given_step_one,
                When_step_two_throwing_exception,
                Then_step_three));

            Assert.That(ex.Message, Is.EqualTo("test"));

            Assert.That(_executedSteps, Is.EqualTo(new[] { "Given_step_one", "When_step_two_throwing_exception" }));
        }

        [Test]
        public void Disposed_feature_should_not_allow_creating_new_scenarios()
        {
            _feature.Dispose();
            Assert.Throws<ObjectDisposedException>(() => _runner.Integrate().NewScenario());
        }

        [Test]
        public void Disposed_feature_should_not_allow_creating_new_runners()
        {
            _feature.Dispose();
            Assert.Throws<ObjectDisposedException>(() => _feature.GetRunner(this));
        }

        [Test]
        public void Disposed_runner_should_allow_retrieving_results()
        {
            _feature.Dispose();
            Assert.DoesNotThrow(() => _feature.GetFeatureResult());
        }

        private void Given_step_one() { _executedSteps.Add("Given_step_one"); }
        private void When_step_two() { _executedSteps.Add("When_step_two"); }
        private void When_step_two_throwing_exception()
        {
            _executedSteps.Add("When_step_two_throwing_exception");
            throw new InvalidOperationException("test");
        }
        private void Then_step_three() { _executedSteps.Add("Then_step_three"); }
    }
}
