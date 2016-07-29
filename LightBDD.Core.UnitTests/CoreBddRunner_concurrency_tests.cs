using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;
using Steps = LightBDD.Core.UnitTests.Helpers.Steps;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_concurrency_tests : Steps
    {
        private readonly int _elementsCount = 1500;
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _runner = TestableBddRunnerFactory.GetRunner(GetType());
        }

        [Test]
        public void Running_scenarios_should_be_thread_safe()
        {
            Enumerable.Range(0, _elementsCount)
                .ToArray()
                .AsParallel()
                .ForAll(Parallel_scenario);

            Assert.That(_runner.Integrate().GetFeatureResult().GetScenarios().Count(), Is.EqualTo(_elementsCount));

            for (int i = 0; i < _elementsCount; ++i)
            {
                var expectedScenarioName = $"Parallel scenario \"{i}\"";
                var expectedSteps = new[]
                {
                    $"GIVEN step with parameter \"{i}\"",
                    $"WHEN step with parameter \"{i}\" and comments",
                    $"THEN step with parameter \"{i}\""
                };
                var expectedComments = new[] { CommentReason, $"{i}" };

                var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().FirstOrDefault(s => s.Info.Name.ToString() == expectedScenarioName);
                Assert.That(scenario, Is.Not.Null, "Missing scenario: {0}", i);

                Assert.That(scenario.GetSteps().Select(s => s.Info.Name.ToString()).ToArray(), Is.EqualTo(expectedSteps), $"In scenario {i}");
                Assert.That(scenario.GetSteps().ElementAt(1).Comments, Is.EqualTo(expectedComments), $"In scenario {i}");
            }
        }

        private void Parallel_scenario(int idx)
        {
            var steps = new[]
            {
                TestStep.CreateAsync(Given_step_with_parameter, idx.ToString()),
                TestStep.CreateAsync(When_step_with_parameter_and_comments, idx),
                TestStep.CreateAsync(Then_step_with_parameter, (double) idx)
            };

            _runner.Test().TestNamedScenario($"Parallel scenario \"{idx}\"", steps);
        }
    }
}