using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Discovery;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class ExecutionPipeline_asynchronous_scenario_tests
{
    private static readonly AsyncLocal<SemaphoreSlim> SemaphoreOut = new();
    private static readonly AsyncLocal<SemaphoreSlim> SemaphoreIn = new();
    class ParallelFixture
    {
        [TestScenario]
        public async Task Coordinated_scenario(int x)
        {
            await TestScenarioBuilder.Current.TestScenario(
                 TestStep.Create(Given_step, x),
                 TestStep.Create(When_step, x),
                 TestStep.Create(Then_step, x));
        }

        private Task Given_step(int x)
        {
            SemaphoreIn.Value!.Release(1);
            return Task.CompletedTask;
        }

        private async Task When_step(int x)
        {
            if (!await SemaphoreOut.Value!.WaitAsync(TimeSpan.FromSeconds(1)))
                throw new TimeoutException();
        }

        private async Task Then_step(int x)
        {
            await Task.Yield();
            ScenarioExecutionContext.CurrentStep.Comment($"{x}");
        }
    }

    [Test]
    public async Task It_should_run_scenarios_in_parallel()
    {
        SemaphoreIn.Value = new SemaphoreSlim(0);
        SemaphoreOut.Value = new SemaphoreSlim(0);

        var methodInfo = typeof(ParallelFixture).GetMethod(nameof(ParallelFixture.Coordinated_scenario));
        var cases = Enumerable.Range(0, 100)
            .Select(i => ScenarioCase.CreateParameterized(typeof(ParallelFixture).GetTypeInfo(), methodInfo, new[] { (object)i }))
            .ToArray();

        var testRun = TestableCoreExecutionPipeline.Default.Execute(cases);
        for (var index = 0; index < cases.Length; index++)
        {
            if (!await SemaphoreIn.Value.WaitAsync(TimeSpan.FromSeconds(1)))
                Assert.Fail($"Scenario failed to start at index={index}");
        }

        SemaphoreOut.Value.Release(cases.Length);
        var result = await testRun;
        result.OverallStatus.ShouldBe(ExecutionStatus.Passed);
        var scenarioResults = result.Features.Single().GetScenarios().ToArray();
        scenarioResults.Length.ShouldBe(cases.Length);

        var expectedNames = Enumerable.Range(0, cases.Length).Select(i => $"Coordinated_scenario [x: \"{i}\"]").ToArray();
        scenarioResults.Select(x => x.Info.Name.ToString()).ShouldBe(expectedNames);

        for (var i = 0; i < cases.Length; ++i)
        {
            var expectedSteps = new[]
            {
                $"Given_step [x: \"{i}\"]",
                $"When_step [x: \"{i}\"]",
                $"Then_step [x: \"{i}\"]"
            };
            var expectedComments = new[] { $"{i}" };

            var scenario = scenarioResults.FirstOrDefault(s => s.Info.Name.ToString() == expectedNames[i]);
            Assert.That(scenario, Is.Not.Null, "Missing scenario: {0}", i);

            Assert.That(scenario.GetSteps().Select(s => s.Info.Name.ToString()).ToArray(), Is.EqualTo(expectedSteps), $"In scenario {i}");
            Assert.That(scenario.GetSteps().ElementAt(2).Comments, Is.EqualTo(expectedComments), $"In scenario {i}");
        }
    }
}