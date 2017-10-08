using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_asynchronous_step_execution_tests : Steps
    {
        private IBddRunner _runner;
        private IFeatureRunner _feature;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureRunnerRepository.GetRunner(GetType());
            _runner = _feature.GetBddRunner(this);
        }

        #endregion

        [Test]
        public async Task It_should_support_asynchronous_processing_up_to_step_level()
        {
            var semaphore = new SemaphoreSlim(0);
            var stepEntered = false;

            var scenarioTask = _runner.Test().TestScenarioAsync(async () =>
            {
                stepEntered = true;
                if (!await semaphore.WaitAsync(TimeSpan.FromSeconds(3)))
                    throw new InvalidOperationException("Await failed");
            });

            Assert.That(stepEntered, Is.True);
            semaphore.Release();
            await scenarioTask;
        }

        [Test]
        public async Task It_should_support_parallel_asynchronous_scenario_processing()
        {
            var elementsCount = 2000;

            var scenarios = Enumerable.Range(0, elementsCount).Select(Parallel_scenario);
            await Task.WhenAll(scenarios);

            Assert.That(_feature.GetFeatureResult().GetScenarios().Count(), Is.EqualTo(elementsCount));

            for (var i = 0; i < elementsCount; ++i)
            {
                var expectedScenarioName = $"Parallel scenario \"{i}\"";
                var expectedSteps = new[]
                {
                    $"GIVEN step with parameter \"{i}\"",
                    $"WHEN step with parameter \"{i}\" and comments",
                    $"THEN step with parameter \"{i}\""
                };
                var expectedComments = new[] { CommentReason, $"{i}" };

                var scenario = _feature.GetFeatureResult().GetScenarios().FirstOrDefault(s => s.Info.Name.ToString() == expectedScenarioName);
                Assert.That(scenario, Is.Not.Null, "Missing scenario: {0}", i);

                Assert.That(scenario.GetSteps().Select(s => s.Info.Name.ToString()).ToArray(), Is.EqualTo(expectedSteps), $"In scenario {i}");
                Assert.That(scenario.GetSteps().ElementAt(1).Comments, Is.EqualTo(expectedComments), $"In scenario {i}");
            }
        }

        private Task Parallel_scenario(int idx)
        {
            var steps = new[]
            {
                TestStep.CreateAsync(Given_step_with_parameter, idx.ToString()),
                TestStep.CreateAsync(When_step_with_parameter_and_comments, idx),
                TestStep.CreateAsync(Then_step_with_parameter, (double) idx)
            };

            return _runner.Test().TestNamedScenarioAsync($"Parallel scenario \"{idx}\"", steps);
        }
    }
}