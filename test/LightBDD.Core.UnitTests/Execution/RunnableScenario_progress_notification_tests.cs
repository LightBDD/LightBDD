using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class RunnableScenario_progress_notification_tests
{
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task It_should_notify_progress_of_scenario(bool successfulScenario)
    {
        CapturingProgressNotifier capturingNotifier = new();

        Task ScenarioMethod(ICoreScenarioStepsRunner _)
        {
            if (!successfulScenario)
                throw new InvalidOperationException("failure");
            return Task.CompletedTask;
        }

        void OnConfigure(LightBddConfiguration cfg) => cfg.ProgressNotifierConfiguration().Append(capturingNotifier);

        var result = await TestableScenarioFactory.Create(OnConfigure)
            .CreateScenario(ScenarioMethod)
            .RunAsync();

        capturingNotifier.Events.Select(e => e.GetType().Name).ShouldBe(new[]
        {
            nameof(ScenarioStarting),
            nameof(ScenarioFinished)
        });

        var startingEvent = capturingNotifier.Events.OfType<ScenarioStarting>().Single();
        var finishedEvent = capturingNotifier.Events.OfType<ScenarioFinished>().Single();
        startingEvent.Scenario.ShouldBeSameAs(result.Info);
        finishedEvent.Result.ShouldBeSameAs(result);
        finishedEvent.Time.Time.ShouldBeGreaterThan(startingEvent.Time.Time);
    }
}