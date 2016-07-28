using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    [Label("Ticket-1")]
    [Label("Ticket-2")]
    public class CoreBddRunner_execution_results_tests
    {
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _runner = TestableBddRunnerFactory.GetRunner(GetType());
        }

        [Test]
        [Label("Label-1")]
        [Label("Label-2")]
        public void Execution_results_should_print_user_friendly_output()
        {
            try
            {
                _runner.Test().TestScenario(
                    Given_step_one,
                    When_step_two,
                    Then_step_three);
            }
            catch { }

            var featureResult = _runner.Integrate().GetFeatureResult();
            Assert.That(featureResult.ToString(), Is.EqualTo("[Ticket-1][Ticket-2] CoreBddRunner execution results tests"));

            var scenarioResult = featureResult.GetScenarios().Single();
            Assert.That(scenarioResult.ToString(), Is.EqualTo("[Label-1][Label-2] Execution results should print user friendly output: Failed (Step 3: reason)"));

            Assert.That(scenarioResult.GetSteps().Select(s => s.ToString()).ToArray(), Is.EqualTo(new[]
            {
                "1/3 Given step one: Passed",
                "2/3 When step two: Passed",
                "3/3 Then step three: Failed (reason)"
            }));
        }

        private void Given_step_one() { }
        private void When_step_two() { }
        private void Then_step_three() { throw new InvalidOperationException("reason"); }
    }
}